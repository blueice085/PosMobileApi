namespace PosMobileApi.Models.Responses
{
    public class SuccessLoginResDto
    {
        public ProfileResDto Profile { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string OTPToken { get; set; }
        public DateTime ExpiryDateTime { get; set; }
    }

    public class ProfileResDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
        public string Status { get; set; }
    }
}
