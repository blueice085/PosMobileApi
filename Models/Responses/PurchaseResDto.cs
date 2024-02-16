namespace PosMobileApi.Models.Responses
{
    public class PurchaseResDto
    {
        public string Id { get; set; }
        public string User { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}
