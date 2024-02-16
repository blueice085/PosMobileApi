using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Data.Entities
{
    public class UserPoints
    {
        [Key]
        public string UserId { get; set; }
        public int Points { get; set; }

        public User User { get; set; }
    }
}
