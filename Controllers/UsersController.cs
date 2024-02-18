using JWT.Serializers.Converters;
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
    public class UsersController : ControllerBase
    {
        private readonly IPurchaseDAL _purchaseDal;
        private readonly IUserPointsDAL _userpointDal;

        public UsersController(IPurchaseDAL purchaseDal,IUserPointsDAL userpointDal)
        {
            _purchaseDal = purchaseDal;
            _userpointDal = userpointDal;
        }

        /// <summary>
        /// Retrieve all Purchases by User (Access Token).
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet("GetUserPurchases")]
        [ProducesResponseType(typeof(BaseResponse<List<PurchaseResDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<PurchaseResDto>>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<List<PurchaseResDto>>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = Constants.ConstantStrings.AUTHACCESSTOKEN)]
        public async Task<IActionResult> GetUserPurchases([FromQuery] string userId)
        {
            return Ok(await _purchaseDal.GetUserPurchases(userId, User));
        }

        /// <summary>
        /// Retrieve all User's Points (Access Token).
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet("GetUserPoints")]
        [ProducesResponseType(typeof(BaseResponse<List<PurchaseResDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<PurchaseResDto>>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<List<PurchaseResDto>>), (int)HttpStatusCode.InternalServerError)]
        [Authorize(AuthenticationSchemes = Constants.ConstantStrings.AUTHACCESSTOKEN)]
        public async Task<IActionResult> GetUserPoints([FromQuery] string userId)
        {
            return Ok(await _userpointDal.GetUserPoints(userId));
        }
    }
}
