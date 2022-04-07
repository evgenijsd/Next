using Next2.ENums;
using Prism.Mvvm;

namespace Next2.Helpers
{
    public class SpoilerItem : BindableBase
    {
        public ESubmenuItemsModifactions State { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public string SelectedImagePath { get; set; }

        public bool CanShowDot { get; set; }
    }
}
