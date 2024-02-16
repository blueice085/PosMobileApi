namespace PosMobileApi.Data.Entities
{
    public class Purchase
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
