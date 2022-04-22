using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class CustomerModel : IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        public int Points { get; set; }

        public int Rewards { get; set; }

        public int GiftCardCount { get; set; }

        public float GiftCardTotal { get; set; }
    }
}
