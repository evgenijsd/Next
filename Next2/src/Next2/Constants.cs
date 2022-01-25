using System;
namespace Application
{
    public static class Constants
    {
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
