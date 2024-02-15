using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using PosMobileApi.Constants;
using PosMobileApi.Data;
using PosMobileApi.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PosMobileApi.Handlers
{
    public class AuthHandler
    {
        private readonly RequestDelegate _next;
        private readonly IUow<Context> _uow;
        private IConfiguration _config;
        private readonly IDistributedCache _redisCache;

        public AuthHandler(RequestDelegate next, IConfiguration config, IUow<Context> uow, IDistributedCache redisCache)
        {
            _next = next;
            _config = config;
            _uow = uow;
            _redisCache = redisCache;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var ipAddress = context.Request.Headers["X-Forwarded-For"];
                string host = context.Request.Headers["Host"].ToString();
                if (!host.Contains("localhost"))
                {
                    // Just Remove For Staging

                    // Int64 ipResult = isValidIP(ipAddress);
                    // if (ipResult == 0)
                    // {
                    //     context.Response.StatusCode = 403;
                    //     context.Response.Headers.Add("IPAddress", ipAddress);
                    //     return;
                    // }
                }

                var authToken = context.Request.Headers["Authorization"];
                if (authToken.Count() != 0)
                {
                    var splitValue = authToken.FirstOrDefault().Split(" ");
                    if (splitValue[0].ToLower() == "bearer")
                    {

                        var token = splitValue[1];
                        var principal = GetPrincipalFromExpiredToken(token);
                        if (principal == null)
                        {
                            context.Response.StatusCode = 401;
                            return;
                        }
                        if (principal.HasClaim(c => c.Type == Constants.ConstantStrings.USERID))
                        {
                            var AccountId = principal.Claims.FirstOrDefault(c => c.Type == Constants.ConstantStrings.USERID).Value;
                            var SessionId = principal.Claims.FirstOrDefault(c => c.Type == Constants.ConstantStrings.SESSIONID).Value;

                            if (!await IsValid(AccountId, SessionId))
                            {
                                context.Response.StatusCode = 401;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = 401;
                return;
            }

            await _next(context);
        }

        async Task<bool> IsValid(string AccountId, string SessionId)
        {
            var redisKey = RedisKey.UserSession + AccountId;
            var redisValue = _redisCache.GetString(redisKey);
            if (!string.IsNullOrEmpty(redisValue) && redisValue == SessionId)
            {
                return true;
            }
            else if (await _uow.Repository<UserSession>().ExistAsync(x => x.Id == AccountId && x.SessionId == SessionId))
            {
                try
                {
                    _redisCache.SetString(redisKey, SessionId, new DistributedCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(0, 5, 0)));
                }
                catch (Exception ex) { Console.WriteLine($"Redis Error ::: GetSuccessLoginInfo, {ex.Message}"); }

                return true;
            }
            return false;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var canRead = tokenHandler.CanReadToken(token);
                var canValidate = tokenHandler.CanValidateToken;

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredOTPToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Keys:OTPToken"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var canRead = tokenHandler.CanReadToken(token);
                var canValidate = tokenHandler.CanValidateToken;

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
