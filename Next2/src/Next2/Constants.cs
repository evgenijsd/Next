namespace Next2
{
    public static class Constants
    {
        public const string ROLE_ADMIN = "Admin";

        public static class Formats
        {
            public const string NUMERIC_FORMAT = "00";
            public const string DATE_FORMAT = "{0:h:mm:ss}";
            public const string SHORT_TIME = "{0:h:mm:ss tt}";
            public const string LONG_DATE = "{0:dddd, d MMMM yyyy}";
            public const string PRICE_FORMAT = "{0:0.00}";
            public const string POINT_FORMAT = "{0:D} pt";
            public const string CASH_FORMAT = "$ {0:#,0.#0}";
            public const string PERCENT_FORMAT = "{0} %";
            public const string LONG_DATE_FORMAT = "MMM dd yyyy / hh:mm tt";
            public const string PHONE_MASK = "(_ _ _) _ _ _-_ _ _ _";
        }

        public static class Analytics
        {
            public const string IOSKey = "7f9810cd-b473-4757-a025-5fa4e0429479";
            public const string AndroidKey = "4eeb551b-15f4-4ced-a0b2-e08a2d3e8798";
        }

        public static class Limits
        {
            public const int TOAST_DURATION = 3;
            public const int SERVER_RESPONCE_DELAY = 100;
            public const int LOGIN_LENGTH = 6;
            public const int MAX_PERCENTAGE = 100;
            public const int MIN_YEAR = 1900;
            public const int MAX_YEAR = 2032;
            public const int DAYS_IN_CALENDAR = 42;
            public const int MAXIMUM_DISCHARGE_NUMBER = 9;
            public const int MAX_QUERY_LENGTH = 100;
            public const int PHONE_LENGTH = 14;
            public const int MAX_NAME_LENGTH = 80;
            public const int MIN_HOUR = 1;
            public const int MAX_HOUR = 12;
            public const int MIN_MINUTE = 0;
            public const int MAX_MINUTE = 59;
        }

        public static class Navigations
        {
            public const string SEARCH = "SEARCH";
            public const string ORDER_ID = "ORDER_ID";
            public const string SEARCH_QUERY = "SEARCH_QUERY";
            public const string SEARCH_MEMBER = "SEARCH_MEMBER";
            public const string SEARCH_CUSTOMER = "SEARCH_CUSTOMER";
            public const string SEARCH_RESERVATION = "SEARCH_RESERVATION";
            public const string FUNC = "FUNC";
            public const string REFRESH_ORDER = "REFRESH_ORDER";
            public const string CURRENT_ORDER = "CURRENT_ORDER";
            public const string BONUS = "BONUS";
            public const string TAX_OFF = "TAX_OFF";
            public const string CATEGORY = "CATEGORY";
            public const string SWITCH_PAGE = "SWITCH_PAGE";
            public const string SEATS = "SEATS";
            public const string REWARD = "REWARD";
            public const string IS_REWARD_APPLIED = "IS_REWARD_APPLIED";
            public const string GO_TO_REWARDS_POINTS = "GO_TO_REWARDS_POINTS";
            public const string INPUT_VALUE = "INPUT_VALUE";
            public const string PLACEHOLDER = "PLACEHOLDER";
            public const string DELETE_DISH = "DELETE_DISH";
            public const string TOTAL_SUM = "TOTAL_SUM";
            public const string PAYMENT_COMPLETE = "PAYMENT_COMPLETE";
            public const string SIGNATURE = "SIGNATURE";
            public const string DISH_MODIFIED = "DISH_MODIFIED";
            public const string TIP_VALUE = "TIP_VALUE";
            public const string TIP_ITEMS = "TIP_ITEMS";
            public const string TIP_TYPE = "TIP_TYPE";
            public const string GIFT_CARD_FOUNDS = "GIFT_CARD_FOUNDS";
            public const string GIFT_CARD_ADDED = "GIFT_CARD_ADDED";
            public const string CONFIRMED_APPLY_REWARD = "CONFIRM_APPLY_REWARD";
            public const string EMPLOYEE_ID = "EMPLOYEE_ID";
            public const string LOGOUT = "LOGOUT";
            public const string ORDER = "ORDER";
            public const string IS_FIRST_ORDER_INIT = "IS_FIRST_ORDER_INIT";
        }

        public static class Validators
        {
            public const string TEXT = @"[^\w\s]";
            public const string NAME = @"[^a-zA-Z\s]";
            public const string WORD = @"[a-z]+";
            public const string CUSTOMER_NAME = @"^([a-z]+(([,.]? ?)|[-']?))+$";
            public const string PASCAL_CASE = @"\b[A-Z]{1}[a-z\d]*\b";
            public const string NUMBER = @"[\D]";
            public const string PHONE = @"^\d{10}$";
            public const string PHONE_MASK = @"\([0-9]{3}\) [0-9]{3}-[0-9]{4}$";
            public const string PHONE_MASK2 = "(XXX) XXX-XXXX";
            public const string CHECK_NUMBER = @"^[\d]";
            public const string EMAIL = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
           @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        }

        public static class DialogParameterKeys
        {
            public const string MESSAGE = "MESSAGE";
            public const string MODEL = "MODEL";
            public const string UPDATE = "UPDATE";
            public const string TITLE = "TITLE";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string CONDITION = "CONDITION";
            public const string OK_BUTTON_TEXT = "OK_BUTTON_TEXT";
            public const string CANCEL_BUTTON_TEXT = "CANCEL_BUTTON_TEXT";
            public const string CONFIRM_MODE = "CONFIRM_MODE";
            public const string ACCEPT = "ACCEPT";
            public const string CANCEL = "CANCEL";
            public const string PORTIONS = "PORTIONS";
            public const string ACTION_ON_DISHES = "ACTION_ON_DISHES";
            public const string REMOVAL_SEAT = "REMOVAL_SEAT";
            public const string SEAT_NUMBERS_OF_CURRENT_ORDER = "SEAT_NUMBERS_OF_CURRENT_ORDER";
            public const string DESTINATION_SEAT_NUMBER = "DESTINATION_SEAT_NUMBER";
            public const string SEATS = "SEATS";
            public const string SPLIT_GROUPS = "SPLIT_GROUPS";
            public const string ORDER_NUMBER = "ORDER_NUMBER";
            public const string CUSTOMER = "CUSTOMER";
            public const string OK_BUTTON_BACKGROUND = "OK_BUTTON_BACKGROUND";
            public const string OK_BUTTON_TEXT_COLOR = "OK_BUTTON_TEXT_COLOR";
            public const string TIP_VALUE_DIALOG = "TIP_VALUE_DIALOG";
            public const string PAID_ORDER_BINDABLE_MODEL = "PAID_ORDER_BINDABLE_MODEL";
            public const string PAYMENT_COMPLETE = "PAYMENT_COMPLETE";
            public const string GIFT_CARD_ADDED = "GIFT_CARD_ADDED";
            public const string GIFT_CARD_FOUNDS = "GIFT_CARD_FOUNDS";
            public const string DISH = "DISH";
            public const string DISCOUNT_PRICE = "DISCOUNT_PRICE";
        }

        public static class API
        {
#if RELEASE
            public const string HOST_URL = "http://139.59.208.79";
#elif STAGE
            public const string HOST_URL = "http://139.59.208.79";
#elif DEV
            public const string HOST_URL = "http://139.59.208.79";
#else
            public const string HOST_URL = "http://139.59.208.79";
#endif
            public const int TOKEN_EXPIRATION_TIME = 12;
            public const int REQUEST_TIMEOUT = 7;
        }

        public static class StatusCode
        {
            public const string UNAUTHORIZED = "Unauthorized";
            public const string SOCKET_CLOSED = "Socket closed";
            public const string BAD_REQUEST = "BadRequest";
            public const string INTERNAL_SERVER_ERROR = "InternalServerError";
        }
    }
}
