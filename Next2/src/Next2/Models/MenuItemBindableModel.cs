using Next2.Enums;
using Next2.Interfaces;
using Next2.ViewModels;
using Prism.Mvvm;

namespace Next2.Models
{
    public class MenuItemBindableModel : BindableBase
    {
        public EMenuItems State { get; set; }

        public bool IsSelected { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public BaseViewModel ViewModel { get; set; }
    }
}