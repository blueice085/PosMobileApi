namespace PosMobileApi.Constants
{
    public class EnumCollections
    {
        public enum UserStatus
        {
            Active,
            Inactive
        }

        public enum DeviceType
        {
            iOS,
            Android
        }

        public enum SocialProvider
        {
            Apple,
            Google,
            Facebook
        }

        public enum Template
        {
            AppleCallBack
        }

        public enum AudioAlbumType
        {
            Album,
            Single,
            EP
        }

        public struct ExpiryType
        {
            public const string Week = "Week";
            public const string Month = "Month";
            public const string Year = "Year";
        }

        public enum PaymentType
        {
            GIFTCARD,
            MPT,
            GOOGLEPAY,
            APPLEPAY,
            KBZPAY,
            CARD,
            DIGITAL,
            MUSICZONE,
            INFLUENCERCODE,
            DATAPACK
        }

        public enum SubscriptionStatus
        {
            Inactive, Active, Pending, Expired, Suspended, Blocked
        }

        public enum SearchType
        {
            General, Artist
        }
        public enum PushNotificationType
        {
            Album,
            Url
        }
        public enum Platform
        {
            Android = 1,
            IOS = 2
        }
        public enum Configurations
        {
            AudioExtensions,
            VideoExtensions,
            MusicSystemSession,
            MusicCloudSession,
            ServiceCharges,
            IOSVersionNumber,
            IOSIsForceUpdate,
            IOSIsMaintenance,
            AppStoreUrl,
            AndroidVersionCode,
            AndroidVersionNumber,
            AndroidIsForceUpdate,
            AndroidIsMaintenance,
            GooglePlayStoreUrl,
            DisabledMPT,
            MPTUrl,
            DisabledFacebook,
            HasLimitToListen,
            MaxListenPerDay
        }
    }
}
