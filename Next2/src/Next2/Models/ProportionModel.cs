using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class ProportionModel : IBaseApiModel
    {
        public ProportionModel()
        {
        }

        public ProportionModel(ProportionModel proportion)
        {
            Id = proportion.Id;
            SetId = proportion.SetId;
            Title = proportion.Title;
            Price = proportion.Price;
        }

        public Guid Id { get; set; }

        public int SetId { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }
    }
}
