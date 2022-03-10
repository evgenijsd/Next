namespace Next2
{
    public static class Constants
    {
        public const string LONG_DATE_FORMAT = "MMM dd yyyy / hh:mm tt";

        public const float TAX_PERCENTAGE = 20;

        public const int LOGIN_PASSWORD_LENGTH = 6;

        public const int SERVER_RESPONCE_DELAY = 100;

        public const string PRICE_FORMAT = "{0:0.00}";

        public const string DEFAULT_CULTURE = "en-US";

        public const int NUMBER_OF_AVAILABLE_SEATS = 10;

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

        public static class Navigations
        {
            public const string SEARCH = nameof(SEARCH);
            public const string FUNC = nameof(FUNC);
            public const string REFRESH_ORDER = nameof(REFRESH_ORDER);
            public const string IS_LAST_USER_LOGGED_OUT = nameof(IS_LAST_USER_LOGGED_OUT);
            public const string CATEGORY = nameof(CATEGORY);
            public const string ADMIN = nameof(ADMIN);
        }

        public static class Validators
        {
            public const string TEXT = @"[^\w\s]";
            public const string NAME = @"[^a-zA-Z\s]";
            public const string NUMBER = @"[\D]";
            public const string CHECK_NUMBER = @"^[\d]";
            public const string EMAIL = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
           @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        }

        public static class DialogParameterKeys
        {
            public const string MESSAGE = nameof(MESSAGE);
            public const string MODEL = nameof(MODEL);
            public const string TITLE = nameof(TITLE);
            public const string OK_BUTTON_TEXT = nameof(OK_BUTTON_TEXT);
            public const string CANCEL_BUTTON_TEXT = nameof(CANCEL_BUTTON_TEXT);
            public const string ACCEPT = nameof(ACCEPT);
            public const string CANCEL = nameof(CANCEL);
            public const string SET = nameof(SET);
            public const string PORTIONS = nameof(PORTIONS);
        }
    }
}
