namespace PosMobileApi.Data.Entities
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsAlcohol { get; set; }
    }
}
