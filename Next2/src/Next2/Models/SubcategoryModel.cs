using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class SubcategoryModel : IEntityBase
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; }
    }
}
