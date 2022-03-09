using Next2.Interfaces;

namespace Next2.Models
{
    public class PortionModel : IBaseModel
    {
        public int Id { get; set; }

        public int SetId { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }
    }
}
