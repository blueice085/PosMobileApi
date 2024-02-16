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
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseDAL _dal;

        public PurchasesController(IPurchaseDAL dal)
        {
            _dal = dal;
        }

        /// <summary>
        /// Retrieve all Purchases by User (Access Token).
        /// </summary>
        /// <param name="model"></param>
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
            return Ok(await _dal.GetUserPurchases(userId, User));
        }
    }
}
