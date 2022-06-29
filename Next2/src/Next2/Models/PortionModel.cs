using Next2.Interfaces;

namespace Next2.Models
{
    public class PortionModel : IBaseModel
    {
        public PortionModel()
        {
        }

        public PortionModel(PortionModel portion)
        {
            Id = portion.Id;
            SetId = portion.SetId;
            Title = portion.Title;
            Price = portion.Price;
        }

        public int Id { get; set; }

        public int SetId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }
    }
}