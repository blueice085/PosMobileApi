using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PosMobileApi.Constants;
using PosMobileApi.DALs;
using PosMobileApi.Data;
using PosMobileApi.Data.Entities;
using System.Diagnostics;
using static Dapper.SqlMapper;

namespace PosMobileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IUow<Context> _uow;

        public JobController(IUow<Context> uow)
        {
            _uow = uow;
        }

        [HttpPost("Enqueue")]
        public ActionResult Enqueue()
        {
            var jobId = BackgroundJob.Enqueue(() => Debug.WriteLine("ENQUEUE: Fire-and-forget!"));
            //var jobId = BackgroundJob.Enqueue(() => SayHello());
            return Ok(jobId);
        }

        [HttpPost("Schedule")]
        public ActionResult Schedule()
        {
            var jobId = BackgroundJob.Schedule(() => Debug.WriteLine("SCHEDULE: Delayed!"), TimeSpan.FromMinutes(1));
            return Ok(jobId);
        }

        [HttpPost("Recurring")]
        public ActionResult Recurring()
        {
            RecurringJob.AddOrUpdate("myrecurringjob", () => Debug.WriteLine("Recurring!"), Cron.Minutely);
            return Ok();
        }

        [HttpPost("Continuations")]
        public ActionResult Continuations()
        {
            var jobId = BackgroundJob.ContinueJobWith("1", () => Debug.WriteLine("Continuation!"));
            return Ok(jobId);
        }

        /// <summary>
        /// Create a schedule task that will run at 12:00 AM  (Basic Token).
        /// The task will calculate points got and add to user 
        /// </summary>
        [HttpPost("ProcessUserPoints")]
        [Authorize(AuthenticationSchemes = ConstantStrings.AUTHBASIC)]
        public ActionResult ProcessUserPoints()
        {
            var currentDate = DateTime.UtcNow.Date;
            var scheduleDateTime = currentDate.Date.AddDays(1);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            BackgroundJob.Schedule(() => CalculateAndAddMemberPoints(currentDate, scheduleDateTime), dateTimeOffset);

            return Ok();
        }

        private void CalculateAndAddMemberPoints(DateTime from, DateTime to)
        {
            var userPurchase = _uow.Repository<Purchase>()
                .Query(x => x.Date >= from && x.Date <= to && x.Product.IsAlcohol == false, include: x => x.Include(y => y.Product))
                .GroupBy(x => x.UserId)
                .ToList();

            foreach (var group in userPurchase)
            {
                var userId = group.Key;

                decimal total = 0;
                foreach (var item in group)
                {
                    total += item.Quantity * item.Product.Price;
                }
                int pointToAdd = (int)(total / 10);

                var userPoints = _uow.Repository<UserPoints>().Get(userId);

                if (userPoints is not null)
                {
                    userPoints.Points = userPoints.Points + pointToAdd;
                    _uow.Repository<UserPoints>().Update(userPoints);
                    _uow.SaveChanges();
                }
                else
                {
                    userPoints = new UserPoints { Id = userId, Points = pointToAdd };
                    _uow.Repository<UserPoints>().Add(userPoints);
                    _uow.SaveChanges();
                }

            }
        }

        private void SayHello()
        {
            Debug.WriteLine("HELLO WORLD !");
        }
    }
}
