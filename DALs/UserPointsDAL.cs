using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PosMobileApi.Constants;
using PosMobileApi.Data;
using PosMobileApi.Data.Entities;
using PosMobileApi.Models.Requests;
using PosMobileApi.Models.Responses;
using PosMobileApi.Services;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Pipelines.Sockets.Unofficial.SocketConnection;
using static PosMobileApi.Constants.EnumCollections;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PosMobileApi.DALs
{
    public interface IUserPointsDAL
    {
        Task<BaseResponse<UserPoints>> GetUserPoints(string userId);
    }

    public class UserPointsDAL : IUserPointsDAL
    {
        private readonly IUow<Context> _uow;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _redisCache;

        public UserPointsDAL(IUow<Context> uow, IConfiguration config, IDistributedCache redisCache)
        {
            _uow = uow;
            _config = config;
            _redisCache = redisCache;
        }

        public async Task<BaseResponse<UserPoints>> GetUserPoints(string userId)
        {
            var redisKey = RedisKey.UserPoints + userId;
            var redisValue = _redisCache.GetString(redisKey);
            if (!string.IsNullOrEmpty(redisValue))
            {
                //Debug.WriteLine("RESULT FROM REDIS CACHE");

                var data = JsonConvert.DeserializeObject<UserPoints>(redisValue);
                return new BaseResponse<UserPoints>
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = HttpStatusCode.OK.ToString(),
                    Data = data
                };
            }

            var userPoints = await _uow.Repository<UserPoints>().GetAsync(userId);

            try { _redisCache.SetString(redisKey, JsonConvert.SerializeObject(userPoints)); }//, new DistributedCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(0, 15, 0))
            catch (Exception ex) { Console.WriteLine($"Redis Error ::: GetUserPoints, {ex.Message}"); }

            return new BaseResponse<UserPoints>
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = userPoints
            };
        }
    }
}
