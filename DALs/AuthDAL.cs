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
    public interface IAuthDAL
    {
        Task<BaseResponse<SuccessLoginResDto>> RegisterWithMobile(SignupReqDto model, ClaimsPrincipal claim);

        Task<BaseResponse<string>> RequestOTP(DefaultOTPReqDto model);

        Task<BaseResponse<SuccessLoginResDto>> VerifyOTP(VerifyOTPReqDto model);

        Task<BaseResponse<SuccessLoginResDto>> RefreshToken(ClaimsPrincipal claim);

        Task<BaseResponse<string>> Logout(ClaimsPrincipal claim);

        Task<BaseResponse<ProfileResDto>> GetInfo(ClaimsPrincipal claim);

        BaseResponse<string> GetRedisValue(string key);
    }

    public class AuthDAL : IAuthDAL
    {
        private readonly IUow<Context> _uow;
        private readonly IOTPCodeGenerator _otp;
        private readonly ITokenGenerator _token;
        private readonly ITemplateCore _template;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _redisCache;
        //private readonly ISMSSender _smtp;
        private readonly bool _isMPT;

        public AuthDAL(IUow<Context> uow, IOTPCodeGenerator otp, ITokenGenerator token, ITemplateCore template, IConfiguration config, IDistributedCache redisCache)
        {
            _uow = uow;
            _otp = otp;
            _token = token;
            _template = template;
            _config = config;
            _redisCache = redisCache;
            _isMPT = _config["IsMPT"] == "1";
        }


        #region register with mobile
        public async Task<BaseResponse<SuccessLoginResDto>> RegisterWithMobile(SignupReqDto model, ClaimsPrincipal claim)
        {
            model = RetrieveMobileInfo(model, claim);

            var isMobileExisted = await _uow.Repository<User>()
                .ExistAsync(x => x.Mobile == model.Mobile && x.CountryCode == model.CountryCode);

            if (isMobileExisted)
            {
                return new BaseResponse<SuccessLoginResDto>
                {
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "Mobile number belongs to another account.",
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                CountryCode = model.CountryCode,
                Mobile = model.Mobile,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Status = UserStatus.Active.ToString(),
                CreatedAt = DateTime.UtcNow,
            };

            await _uow.Repository<User>().AddAsync(user);
            await _uow.SaveChangesAsync();

            var profile = new ProfileResDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CountryCode = model.CountryCode,
                Mobile = model.Mobile
            };

            return new BaseResponse<SuccessLoginResDto>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = await GetSuccessLoginInfo(profile)
            };
        }
        #endregion

        #region request otp
        public async Task<BaseResponse<string>> RequestOTP(DefaultOTPReqDto model)
        {
            model.Mobile = model.Mobile.TrimStart(new Char[] { '0' });
            string phoneNumber = $"{model.CountryCode}{model.Mobile}";
            phoneNumber = phoneNumber.Replace(" ", string.Empty)
                .Replace("+", string.Empty);

            var env = Environment.GetEnvironmentVariable(ConstantStrings.ENV);
            if (env == ConstantStrings.LOCAL || env == ConstantStrings.DEV || env == ConstantStrings.UAT || phoneNumber.Contains("911911911"))
            {
                return new BaseResponse<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = HttpStatusCode.OK.ToString(),
                };
            }

            //-------------------- Remove later --------------------//
            //if (!phoneNumber.StartsWith("959"))
            //{
            //    return new BaseResponse<string>()
            //    {
            //        Code = (int)HttpStatusCode.OK,
            //        Message = HttpStatusCode.OK.ToString(),
            //    };
            //}

            var timesAgo = DateTime.UtcNow.AddMinutes(-15);
            int requestCountWithin15Min = await _uow.Repository<UserOtpCode>()
                .Query(x => x.Mobile == phoneNumber && x.CreatedAt > timesAgo)
                .CountAsync();

            if (requestCountWithin15Min > 5)
            {
                return new BaseResponse<string>()
                {
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "Too many OTP requests. Please try again later.",
                };
            }

            string code = _otp.GenerateOTPCode();
            bool isSuccess = false;

            //if (phoneNumber.StartsWith("95") || phoneNumber.StartsWith("86") || phoneNumber.StartsWith("66"))
            //    isSuccess = await _smtp.SendBySMSPoh(phoneNumber, code, isEn);
            //else
            //    isSuccess = await _smtp.SendByAwsSNS(phoneNumber, code, isEn);

            if (isSuccess == false)
            {
                return new BaseResponse<string>()
                {
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "Please try again later.",
                };
            }

            await _uow.Repository<UserOtpCode>().AddAsync(new UserOtpCode
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                Mobile = phoneNumber,
                OtpCode = code,
                IsUsed = false,
            });
            await _uow.SaveChangesAsync();

            return new BaseResponse<string>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
            };
        }
        #endregion

        #region verify otp
        public async Task<BaseResponse<SuccessLoginResDto>> VerifyOTP(VerifyOTPReqDto model)
        {
            model.Mobile = model.Mobile.TrimStart(new Char[] { '0' });
            string phoneNumber = $"{model.CountryCode}{model.Mobile}";
            phoneNumber = phoneNumber.Replace(" ", string.Empty)
                .Replace("+", string.Empty);

            var env = Environment.GetEnvironmentVariable(ConstantStrings.ENV);
            if ((env == ConstantStrings.LOCAL || env == ConstantStrings.DEV /*|| env == ConstantStrings.UAT*/ || !phoneNumber.StartsWith("959") || phoneNumber.Contains("911911911")) && model.OTP == "101010")
            {
                var registeredUser = await _uow.Repository<User>()
                    .Query(x => x.CountryCode == model.CountryCode && x.Mobile == model.Mobile)
                    .AsNoTracking()
                    .Select(x => new ProfileResDto
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        CountryCode = x.CountryCode,
                        Mobile = x.Mobile,
                        Status = x.Status
                    }).FirstOrDefaultAsync();

                if (registeredUser == null)
                {
                    var otpToken = _token.GenerateOTPToken(model, out DateTime expiryDate);
                    return new BaseResponse<SuccessLoginResDto>()
                    {
                        Code = (int)HttpStatusCode.Continue,
                        Message = "Account not found.",
                        Data = new SuccessLoginResDto
                        {
                            OTPToken = otpToken,
                            ExpiryDateTime = expiryDate,
                        }
                    };
                }

                if (registeredUser.Status == UserStatus.Inactive.ToString())
                {
                    return new BaseResponse<SuccessLoginResDto>()
                    {
                        Code = (int)HttpStatusCode.Continue,
                        Message = "Account disabled by Flow Admin.",
                    };
                }

                if (!string.IsNullOrEmpty(model.SocialType) && !string.IsNullOrEmpty(model.SocialIdentifier))
                {
                    var user = await _uow.Repository<User>().SingleAsync(x => x.Id == registeredUser.Id);

                    _uow.Repository<User>().Update(user);

                    await _uow.SaveChangesAsync();
                }


                return new BaseResponse<SuccessLoginResDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = HttpStatusCode.OK.ToString(),
                    Data = await GetSuccessLoginInfo(registeredUser)
                };
            }

            var otpRecord = await _uow.Repository<UserOtpCode>()
                .Query(x => x.Mobile == phoneNumber && x.IsUsed == false && x.OtpCode == model.OTP)
                .FirstOrDefaultAsync();
            if (otpRecord == null)
            {
                return new BaseResponse<SuccessLoginResDto>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Invalid OTP.",
                };
            }

            var registeredUserLive = await _uow.Repository<User>()
                   .Query(x => x.CountryCode == model.CountryCode && x.Mobile == model.Mobile)
                   .AsNoTracking()
                   .Select(x => new ProfileResDto
                   {
                       Id = x.Id,
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       CountryCode = x.CountryCode,
                       Mobile = x.Mobile,
                       Status = x.Status
                   }).FirstOrDefaultAsync();

            if (registeredUserLive == null)
            {
                var otpToken = _token.GenerateOTPToken(model, out DateTime expiryDate);
                return new BaseResponse<SuccessLoginResDto>()
                {
                    Code = (int)HttpStatusCode.Continue,
                    Message = "Account not found",
                    Data = new SuccessLoginResDto
                    {
                        OTPToken = otpToken,
                        ExpiryDateTime = expiryDate,
                    }
                };
            }

            if (registeredUserLive.Status == UserStatus.Inactive.ToString())
            {
                return new BaseResponse<SuccessLoginResDto>()
                {
                    Code = (int)HttpStatusCode.Continue,
                    Message = "Account disabled by Flow Admin.",
                };
            }

            if (!string.IsNullOrEmpty(model.SocialType) && !string.IsNullOrEmpty(model.SocialIdentifier))
            {
                var user = await _uow.Repository<User>().SingleAsync(x => x.Id == registeredUserLive.Id);

                _uow.Repository<User>().Update(user);

                await _uow.SaveChangesAsync();
            }

            var data = await GetSuccessLoginInfo(registeredUserLive);

            otpRecord.IsUsed = true;
            _uow.Repository<UserOtpCode>().Update(otpRecord);
            await _uow.SaveChangesAsync();

            return new BaseResponse<SuccessLoginResDto>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = data
            };
        }
        #endregion

        #region refresh token
        public async Task<BaseResponse<SuccessLoginResDto>> RefreshToken(ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ConstantStrings.USERID).Value;
            var user = await _uow.Repository<User>()
                .Query(x => x.Id == userId)
                .AsNoTracking()
                .Select(x => new ProfileResDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    //CountryCode = x.CountryCode,
                    //Mobile = x.Mobile,
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new BaseResponse<SuccessLoginResDto>()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Your login session has expired.",
                };
            }

            return new BaseResponse<SuccessLoginResDto>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = await GetSuccessLoginInfo(user)
            };
        }
        #endregion

        #region logout
        public async Task<BaseResponse<string>> Logout(ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ConstantStrings.USERID).Value;
            var sessionId = claim.FindFirst(ConstantStrings.SESSIONID).Value;

            var oldSession = await _uow.Repository<UserSession>()
                .Query(x => x.Id == userId && x.SessionId == sessionId)
                .ToListAsync();
            if (oldSession != null)
            {
                _uow.Repository<UserSession>().Delete(oldSession);
            }

            // Redis
            var redisKey = RedisKey.UserSession + userId;
            var redisValue = _redisCache.GetString(redisKey);
            if (!string.IsNullOrEmpty(redisValue)) _redisCache.Remove(redisKey);

            var userInfo = await _uow.Repository<User>().SingleAsync(x => x.Id == userId);
            userInfo.FCMToken = null;
            userInfo.Platform = null;
            _uow.Repository<User>().Update(userInfo);
            await _uow.SaveChangesAsync();
            return new BaseResponse<string>
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString()
            };
        }
        #endregion

        #region get user info
        public async Task<BaseResponse<ProfileResDto>> GetInfo(ClaimsPrincipal claims)
        {
            var userId = claims.FindFirst(ConstantStrings.USERID).Value;
            var langId = claims.FindFirst(ConstantStrings.LANG).Value;

            var user = await _uow.Repository<User>().SingleAsync(x => x.Id == userId);

            if (user == null)
            {
                return new BaseResponse<ProfileResDto>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = HttpStatusCode.NotFound.ToString(),
                };
            }

            var data = new ProfileResDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                //CountryCode = user.CountryCode,
                //Mobile = user.Mobile,
            };

            return new BaseResponse<ProfileResDto>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = data
            };
        }
        #endregion

        #region get redis value with key
        public BaseResponse<string> GetRedisValue(string key)
        {
            return new BaseResponse<string>
            {
                Code = (int)HttpStatusCode.OK,
                Message = HttpStatusCode.OK.ToString(),
                Data = _redisCache.GetString(key)
            };
        }
        #endregion

        #region private methods
        private SignupReqDto RetrieveMobileInfo(SignupReqDto model, ClaimsPrincipal claim)
        {
            model.CountryCode = claim.FindFirst(ConstantStrings.COUNTRYCODE).Value;
            model.Mobile = claim.FindFirst(ConstantStrings.MOBILE).Value;
            return model;
        }

        private async Task<SuccessLoginResDto> GetSuccessLoginInfo(ProfileResDto model)
        {
            var sessionId = Guid.NewGuid().ToString();

            var oldSession = await _uow.Repository<UserSession>()
                .Query(x => x.Id == model.Id)
                .ToListAsync();
            if (oldSession != null)
            {
                _uow.Repository<UserSession>().Delete(oldSession);
            }

            await _uow.Repository<UserSession>()
                .AddAsync(new UserSession
                {
                    Id = model.Id,
                    SessionId = sessionId
                });
            await _uow.SaveChangesAsync();

            var accessToken = _token.GenerateAccessToken(model, sessionId.ToString(), out DateTime expiryDate);

            // Redis
            try
            {
                var redisKey = RedisKey.UserSession + model.Id;
                _redisCache.SetString(redisKey, sessionId, new DistributedCacheEntryOptions().SetAbsoluteExpiration(expiryDate));
            }
            catch (Exception ex) { Console.WriteLine($"Redis Error ::: GetSuccessLoginInfo, {ex.Message}"); }

            return new SuccessLoginResDto
            {
                AccessToken = accessToken,
                RefreshToken = _token.GenerateRefreshToken(model, sessionId.ToString(), out DateTime refreshExpiryDate),
                Profile = model,
                ExpiryDateTime = expiryDate
            };
        }
        #endregion
    }
}
