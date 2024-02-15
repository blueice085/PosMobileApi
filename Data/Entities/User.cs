namespace PosMobileApi.Data.Entities
{
    public partial class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
        public DateTime? Birthday { get; set; }
        public string Status { get; set; }
        public string FCMToken { get; set; }
        public short? Platform { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
