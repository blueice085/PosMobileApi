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
    public interface IUserCuponsDAL
    {
        Task<BaseResponse<List<UserCupons>>> GetUserCupons(string userId);
    }

    public class UserCuponsDAL : IUserCuponsDAL
    {
        private readonly IUow<Context> _uow;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _redisCache;

        public UserCuponsDAL(IUow<Context> uow, IConfiguration config, IDistributedCache redisCache)
        {
            _uow = uow;
            _config = config;
            _redisCache = redisCache;
        }

        public async Task<BaseResponse<List<UserCupons>>> GetUserCupons(string userId)
        {
            var userCupons = await _uow.Repository<UserCupons>().Query(x => x.UserId == userId).ToListAsync();

            return new BaseResponse<List<UserCupons>>
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = userCupons
            };
        }
    }
}
