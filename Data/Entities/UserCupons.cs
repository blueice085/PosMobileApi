using static PosMobileApi.Constants.EnumCollections;

namespace PosMobileApi.Data.Entities
{
    public class UserCupons
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public CuponType Cupon { get; set; }
        public int Quantity { get; set; }
        public DateTime ExchangedDate { get; set; }

        public User User { get; set; }
    }
}
