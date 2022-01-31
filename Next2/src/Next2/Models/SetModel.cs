using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class SetModel : IEntityBase
    {
        public int Id { get; set; }

        public int SubcategoryId { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }

        public string ImagePath { get; set; }
    }
}
