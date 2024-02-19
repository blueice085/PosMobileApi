using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PosMobileApi.Constants;
using PosMobileApi.Models;
using PosMobileApi.Models.Responses;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PosMobileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponsController : ControllerBase
    {

        IFirebaseConfig config;
        IFirebaseClient client;

        public CuponsController()
        {
            config = new FirebaseConfig
            {
                AuthSecret = "9yE5lAFZdlDw5xSbgZOElwavEyFHyec5Z3itmt3N",
                BasePath = "https://forposcupons-default-rtdb.asia-southeast1.firebasedatabase.app/"
            };
        }
        /// <summary>
        /// Retrieve all currently available Cupons from Firebase Realtime DB (Basic Token).
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet("GetAvailableCupons")]
        [ProducesResponseType(typeof(BaseResponse<List<CuponAvailable>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CuponAvailable>>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<List<CuponAvailable>>), (int)HttpStatusCode.InternalServerError)]
        //[Authorize(AuthenticationSchemes = ConstantStrings.AUTHBASIC)]
        public async Task<IActionResult> GetAvailableCupons()
        {
            client = new FireSharp.FirebaseClient(config);

            //var data = new CuponAvailable { CuponType = EnumCollections.CuponType.FiveDollars, Quantity = 100 };
            //PushResponse response = client.Push("Cupons/", data);

            //data.Id = response.Result.name;
            //SetResponse setResponse = client.Set("Cupons/" + data.Id, data);

            FirebaseResponse response = client.Get("Cupons");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<CuponAvailable>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<CuponAvailable>(((JProperty)item).Value.ToString()));
                }
            }

            return Ok(list);
        }
    }
}
