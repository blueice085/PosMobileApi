using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Models.Requests
{
    public class OTPReqDto
    {
        //[Required]
        [MaxLength(10)]
        public string CountryCode { get; set; }

        //[Required]
        [MaxLength(15)]
        public string Mobile { get; set; }
    }

    public class DefaultOTPReqDto
    {
        [Required]
        [MaxLength(10)]
        public string CountryCode { get; set; }

        [Required]
        [MaxLength(15)]
        public string Mobile { get; set; }
    }
}
