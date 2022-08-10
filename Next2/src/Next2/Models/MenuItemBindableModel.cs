using Next2.Enums;
using Next2.ViewModels;
using Prism.Mvvm;

namespace Next2.Models
{
    public class MenuItemBindableModel : BindableBase
    {
        public EMenuItems State { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public BaseViewModel ViewModel { get; set; }
    }
}