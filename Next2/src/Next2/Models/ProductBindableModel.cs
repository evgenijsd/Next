using Next2.Interfaces;
using System.Collections.Generic;

namespace Next2.Models
{
    public class ProductBindableModel : IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<ItemSpoilerModel> Items { get; set; }
    }
}
