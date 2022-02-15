using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class CategoryModel : IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
