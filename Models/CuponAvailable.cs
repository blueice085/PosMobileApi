using static PosMobileApi.Constants.EnumCollections;

namespace PosMobileApi.Models
{
    public class CuponAvailable
    {
        public string Id { get; set; }
        public CuponType CuponType { get; set; }
        public int Quantity { get; set; }
    }
}
