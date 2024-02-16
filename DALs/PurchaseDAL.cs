using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PosMobileApi.Constants;
using PosMobileApi.Data;
using PosMobileApi.Data.Entities;
using PosMobileApi.Models.Requests;
using PosMobileApi.Models.Responses;
using PosMobileApi.Services;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text;
using static PosMobileApi.Constants.EnumCollections;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PosMobileApi.DALs
{
    public interface IPurchaseDAL
    {
        Task<BaseResponse<List<PurchaseResDto>>> GetUserPurchases(string userId, ClaimsPrincipal claim);

        //BaseResponse<string> GetRedisValue(string key);
    }

    public class PurchaseDAL : IPurchaseDAL
    {
        private readonly IUow<Context> _uow;
        private readonly IConfiguration _config;
        //private readonly IDistributedCache _redisCache;

        public PurchaseDAL(IUow<Context> uow, IConfiguration config)
        {
            _uow = uow;
            _config = config;
            //_redisCache = redisCache;
        }

        public async Task<BaseResponse<List<PurchaseResDto>>> GetUserPurchases(string userId, ClaimsPrincipal claim)
        {
            //var userId = claim.FindFirst(ConstantStrings.USERID).Value;

            var data = await _uow.Repository<Purchase>().Query(x => x.UserId == userId)
                .Include(x => x.User).Include(x => x.Product)
                .Select(x => new PurchaseResDto
                {
                    Id = x.Id,
                    User = $"{x.User.FirstName} {x.User.LastName}",
                    Product = x.Product.Name,
                    Quantity = x.Quantity,
                    Date = x.Date
                }).ToListAsync();

            return new BaseResponse<List<PurchaseResDto>>
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = data
            };
        }
    }
}
