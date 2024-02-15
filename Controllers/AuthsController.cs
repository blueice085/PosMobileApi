using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosMobileApi.Constants;
using PosMobileApi.DALs;
using PosMobileApi.Models.Requests;
using PosMobileApi.Models.Responses;
using System.Net;
using System.Text;

namespace PosMobileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthDAL _auth;

        public AuthsController(IAuthDAL auth)
        {
            _auth = auth;
        }

        /// <summary>
        /// Register with mobile (OTP Token).
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="400">BadRequest</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Mobile number or username belongs to another account</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = ConstantStrings.OTPTOKENAUTH)]
        public async Task<IActionResult> Register([FromBody] SignupReqDto model)
        {
            return Ok(await _auth.RegisterWithMobile(model, User));
        }

        /// <summary>
        /// Request otp (Basic Token).
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Too many OTP requests. Please try again later</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost("RequestOTP")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = ConstantStrings.AUTHBASIC)]
        public async Task<IActionResult> RequestOTP([FromBody] DefaultOTPReqDto model)
        {
            return Ok(await _auth.RequestOTP(model));
        }

        /// <summary>
        /// Verify user mobile via otp (Basic Token).
        /// </summary>
        /// <param name="model"></param>
        /// <response code="100">Account not found, can continue account creation</response>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Invalid OTP</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost("VerifyOTP")]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.Continue)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = ConstantStrings.AUTHBASIC)]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPReqDto model)
        {
            return Ok(await _auth.VerifyOTP(model));
        }

        /// <summary>
        /// Regenerate access token via refresh token (Refresh Token).
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Login session has expired</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost("RefreshToken")]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<SuccessLoginResDto>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = ConstantStrings.REFRESHTOKENAUTH)]
        public async Task<IActionResult> RefreshToken()
        {
            return Ok(await _auth.RefreshToken(User));
        }

        /// <summary>
        /// Logout current user from the system (Access Token).
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Payload data need to recheck</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = ConstantStrings.AUTHACCESSTOKEN)]
        public async Task<IActionResult> Logout()
        {
            await _auth.Logout(User);
            return Ok();
        }

        /// <summary>
        /// Get current user info (Access Token).
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">NotFound</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet("GetUserInfo")]
        [ProducesResponseType(typeof(BaseResponse<ProfileResDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<ProfileResDto>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<ProfileResDto>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<ProfileResDto>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = ConstantStrings.AUTHACCESSTOKEN)]
        public async Task<IActionResult> GetInfo()
        {
            return Ok(await _auth.GetInfo(User));
        }

        /// <summary>
        /// Get Redis Value with Key
        /// </summary>
        /// <response code="200">Success</response>
        [HttpGet("Redis")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [Authorize(AuthenticationSchemes = ConstantStrings.AUTHACCESSTOKEN)]
        public IActionResult GetRedisValue([FromQuery] string key)
        {
            return Ok(_auth.GetRedisValue(key));
        }
    }
}
