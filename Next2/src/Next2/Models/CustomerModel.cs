using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class CustomerModel : IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int Points { get; set; }

        public int Rewards { get; set; }

        public int GiftCardCount { get; set; }

        public double GiftCardTotal { get; set; }
    }
}
