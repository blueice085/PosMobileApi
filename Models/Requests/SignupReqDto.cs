using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Models.Requests
{
    public class SignupReqDto : OTPReqDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
    }
}
