using Next2.Interfaces;

namespace Next2.Models
{
    public class OptionModel : IBaseModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Title { get; set; }
    }
}
