using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Models.Requests
{
    public class VerifyOTPReqDto : OTPReqDto
    {
        [Required]
        [MaxLength(6)]
        public string OTP { get; set; }
    }
}
