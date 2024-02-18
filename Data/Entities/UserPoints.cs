using System.ComponentModel.DataAnnotations;

namespace PosMobileApi.Data.Entities
{
    public class UserPoints
    {
        [Key]
        public string Id { get; set; }
        public int Points { get; set; }
    }
}
