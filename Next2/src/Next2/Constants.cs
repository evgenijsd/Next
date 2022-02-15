﻿namespace Next2
{
    public static class Constants
    {
        public const string LONG_DATE_FORMAT = "MMM dd yyyy / hh:mm tt";

        public const int LOGIN_PASSWORD_LENGTH = 6;

        public const int SERVER_RESPONCE_DELAY = 1000;

        public const string DEFAULT_CULTURE = "en-US";

#if RELEASE
        public const string BASE_URL = "dfdfdffd";
#elif STAGE
            public const string BASE_URL = "dfdfdffd";
#elif DEV
        public const string BASE_URL = "dfdfdffd";
#else
        public const string BASE_URL = "dfdfdffd";
#endif
        public static class Analytics
        {
            public const string IOSKey = "7f9810cd-b473-4757-a025-5fa4e0429479";

            public const string AndroidKey = "4eeb551b-15f4-4ced-a0b2-e08a2d3e8798";
        }

        public static class LayoutOrderTabs
        {
            public const double SUMM_ROW_HEIGHT_MOBILE = 65 + 55 + 80;
            public const double OFFSET_MOBILE = 52;
            public const double SUMM_ROW_HEIGHT_TABLET = 75 + 2 + 95;
            public const double OFFSET_TABLET = 70;
            public const double BUTTONS_HEIGHT = 142;
            public const double ROW_HEIGHT = 48 + 2;
        }

        public static class DialogParameterKeys
        {
            public const string SET = nameof(SET);
            public const string CATEGORY = nameof(CATEGORY);
        }
    }
}
