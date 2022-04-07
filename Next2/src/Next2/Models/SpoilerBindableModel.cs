using Next2.Helpers;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Next2.Models
{
    public class SpoilerBindableModel : BindableBase, IBaseModel, ITappable
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public ObservableCollection<SpoilerItem> Items { get; set; }

        public SpoilerItem? SelectedItem { get; set; }

        public ICommand TapCommand { get; set; }
    }
}
