namespace PosMobileApi.Services
{
    public interface IOTPCodeGenerator
    {
        string GenerateOTPCode();
    }

    public class OTPCodeGenerator : IOTPCodeGenerator
    {
        public OTPCodeGenerator()
        {
            //todo: DI uow
        }

        public string GenerateOTPCode()
        {
            string code;
            bool isUnique = false;
            do
            {
                Random random = new();
                code = random.Next(000000, 999999).ToString("000000");
                //todo: to check unique
            } while (isUnique);
            return code;
        }
    }
}
