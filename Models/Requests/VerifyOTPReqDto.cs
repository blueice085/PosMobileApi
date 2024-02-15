using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Models.Requests
{
    public class VerifyOTPReqDto : OTPReqDto
    {
        [Required]
        [MaxLength(6)]
        public string OTP { get; set; }

        [RegularExpression(@"^Google|Facebook|Apple$", ErrorMessage = "Provider only allow Facebook or Google or Apple")]
        public string SocialType { get; set; }

        public string SocialIdentifier { get; set; }
    }
}
