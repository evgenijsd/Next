namespace Next2
{
    public static class Constants
    {
        public const string MEMBERSHIP_TIME_FORMAT = "MMM dd yyyy / hh:mm tt";

        public const int SERVER_RESPONCE_DELAY = 3000;

#if RELEASE
        public const string BASE_URL = "dfdfdffd";
#elif STAGE
        public const string BASE_URL = "dfdfdffd";
#elif DEV
        public const string BASE_URL = "dfdfdffd";
#else
        public const string BASE_URL = "dfdfdffd";
#endif
    }
}
