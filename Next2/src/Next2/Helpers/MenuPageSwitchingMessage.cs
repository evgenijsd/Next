using Next2.Enums;

namespace Next2.Helpers
{
    public class MenuPageSwitchingMessage
    {
        public MenuPageSwitchingMessage(EMenuItems page)
        {
            Page = page;
        }

        #region -- Public properties --

        public EMenuItems Page { get; }

        #endregion
    }
}
