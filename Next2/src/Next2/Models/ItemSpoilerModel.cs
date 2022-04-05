using Next2.ENums;
using System.Windows.Input;

namespace Next2.Models
{
    public class ItemSpoilerModel
    {
        public ESubmenuItemsModifactions State { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public string SelectedImagePath { get; set; }

        public bool CanShowDot { get; set; }
    }
}
