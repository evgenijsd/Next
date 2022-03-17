using Next2.Interfaces;

namespace Next2.Models
{
    public class OptionModel : IBaseModel
    {
        public OptionModel()
        {
        }

        public OptionModel(OptionModel option)
        {
            Id = option.Id;
            ProductId = option.ProductId;
            Title = option.Title;
        }

        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Title { get; set; }
    }
}
