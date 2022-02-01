using Next2.ENums;
using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class MenuItemBindableModel : BindableBase, ISelectable
    {
        public EMenuItems State { get; set; }

        public bool IsSelected { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }
    }
}