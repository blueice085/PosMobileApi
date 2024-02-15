using Microsoft.IdentityModel.Tokens;
using PosMobileApi.Constants;
using PosMobileApi.Models.Requests;
using PosMobileApi.Models.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PosMobileApi.Services
{
    public interface ITokenGenerator
    {
        public string GenerateAccessToken(ProfileResDto model, string uniqueDeviceId, out DateTime expiryDate);
        public string GenerateRefreshToken(ProfileResDto model, string uniqueDeviceId, out DateTime expiryDate);
        public string GenerateOTPToken(VerifyOTPReqDto model, out DateTime expiryDate);
    }

    public class TokenGenerator : ITokenGenerator
    {
        private readonly string Issuer;
        private readonly string Audience;
        private readonly string Key;
        private readonly long accessTokenExpiryInMinutes;
        private readonly long refreshTokenExpiryInDays;
        private readonly long otpTokenExpiryInMinutes;

        public TokenGenerator(IConfiguration config)
        {
            Issuer = config["JWT:Issuer"];
            Audience = config["JWT:Audience"];
            Key = config["JWT:Key"];
            accessTokenExpiryInMinutes = long.Parse(config["JWT:AccessTokenExpiryInMinutes"]);
            refreshTokenExpiryInDays = long.Parse(config["JWT:RefreshTokenExpiryInDays"]);
            otpTokenExpiryInMinutes = long.Parse(config["JWT:OTPTokenExpiryInMinutes"]);
        }

        public string GenerateAccessToken(ProfileResDto model, string uniqueDeviceId, out DateTime expiryDate)
        {
            var claims = new[] {
                //new Claim(ConstantStrings.EMAIL, model.Email),
                new Claim(ConstantStrings.USERID, model.Id),
                new Claim(ConstantStrings.FIRSTNAME, model.FirstName),
                new Claim(ConstantStrings.LASTNAME, model.LastName),
                new Claim(ConstantStrings.MOBILE, string.IsNullOrEmpty(model.Mobile) ? "" : model.Mobile),
                new Claim(ConstantStrings.SESSIONID, uniqueDeviceId),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            expiryDate = DateTime.Now.AddMinutes(accessTokenExpiryInMinutes);

            var token = new JwtSecurityToken(Issuer, Audience,
              claims,
              expires: expiryDate,
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(ProfileResDto model, string uniqueDeviceId, out DateTime expiryDate)
        {
            var claims = new[] {
                new Claim(ConstantStrings.SESSIONID, uniqueDeviceId),
                new Claim(ConstantStrings.USERID, model.Id),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            expiryDate = DateTime.Now.AddDays(refreshTokenExpiryInDays);
            var token = new JwtSecurityToken(Issuer, Audience,
              claims,
              expires: expiryDate,
              signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateOTPToken(VerifyOTPReqDto model, out DateTime expiryDate)
        {
            var claims = new[] {
                new Claim(ConstantStrings.COUNTRYCODE, model.CountryCode),
                new Claim(ConstantStrings.MOBILE, model.Mobile),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            expiryDate = DateTime.Now.AddMinutes(otpTokenExpiryInMinutes);
            var token = new JwtSecurityToken(Issuer, Audience,
              claims,
              expires: expiryDate,
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
