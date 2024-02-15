namespace PosMobileApi.Data.Entities
{
    public partial class UserOtpCode
    {
        public string Id { get; set; }
        public string Mobile { get; set; }
        public string OtpCode { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
