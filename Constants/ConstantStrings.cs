namespace PosMobileApi.Constants
{
    public static class ConstantStrings
    {
        public const string CORSPOLICY = "AppCors";

        public const string AUTHBASIC = "BasicAuth";
        public const string AUTHACCESSTOKEN = "AccessTokenBearer";
        public const string REFRESHTOKENAUTH = "RefreshTokenBearer";
        public const string OTPTOKENAUTH = "OTPTokenBearer";

        public const string EMAIL = "emailAddress";
        public const string USERID = "id";
        public const string NAME = "userName";
        public const string FIRSTNAME = "firstName";
        public const string LASTNAME = "lastName";
        public const string LANG = "languageId";
        public const string MOBILE = "mobile";
        public const string COUNTRYCODE = "countryCode";
        public const string ROLE = "roleId";
        public const string JTI = "jti";
        public const string SESSIONID = "sessionId";

        public const string ENV = "ASPNETCORE_ENVIRONMENT";
        public const string LOCAL = "Local";
        public const string DEV = "Development";
        public const string UAT = "UAT";
        public const string PROD = "Production";

        public const string DEFAULT_EXPIRETIME = "01:00:00";
    }

    public struct Operator
    {
        public const string AND = "AND";
        public const string OR = "OR";
        public const string LIKE = "LIKE";
        public const string EQUAL = "=";
        public const string GREATER = ">";
        public const string GREATEREQUAL = ">=";
        public const string LESSER = "<";
        public const string LESSEREQUAL = "<=";
    }

    public struct RedisKey
    {
        public const string Home = "home";
        public const string UserSession = "user_session_";
        public const string UserPoints = "user_points_";
    }
}
