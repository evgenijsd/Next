using Next2.Interfaces;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class ProductBindableModel : IBaseModel
    {
        public int Id { get; set; }

        public OptionModel SelectedOption { get; set; }

        public ObservableCollection<OptionModel> Options { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float Price { get; set; }
    }
}
