using Next2.Interfaces;

namespace Next2.Models
{
    public class SetBindableModel : IBaseModel
    {
        public int Id { get; set; }

        public int SubcategoryId { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }

        public string ImagePath { get; set; }

        public PortionModel Portion { get; set; }
    }
}
