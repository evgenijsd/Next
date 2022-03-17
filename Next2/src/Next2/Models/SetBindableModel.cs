using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class SetBindableModel : BindableBase, IBaseModel
    {
        public SetBindableModel()
        {
        }

        public SetBindableModel(SetBindableModel set)
        {
            Id = set.Id;
            SubcategoryId = set.SubcategoryId;
            Title = set.Title;
            Price = set.Price;
            ImagePath = set.ImagePath;
            Portion = new();
            Portions = new();
            Products = new();
        }

        public int Id { get; set; }

        public int SubcategoryId { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }

        public string ImagePath { get; set; }

        public PortionModel Portion { get; set; }

        public ObservableCollection<PortionModel> Portions { get; set; }

        public ObservableCollection<ProductBindableModel> Products { get; set; }
    }
}
