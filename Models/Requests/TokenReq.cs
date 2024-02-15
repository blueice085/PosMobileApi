using System;
using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Models.Requests
{
	public class TokenReq
	{
		public string FCMToken { get; set; }
        [Required]
        [RegularExpression(@"^Android|IOS", ErrorMessage = "Can be only one of 'Android | IOS'.")]
        public string Platform { get; set; }
	}
}

