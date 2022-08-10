using Next2.Enums;
using Prism.Mvvm;

namespace Next2.Helpers
{
    public class SpoilerItem : BindableBase
    {
        public ESubmenuItemsModifactions State { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public string SelectedImagePath { get; set; } = string.Empty;

        public bool CanShowDot { get; set; }
    }
}
