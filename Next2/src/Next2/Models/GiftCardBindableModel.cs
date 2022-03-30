using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Next2.Models
{
    public class RewardBindabledModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public EDiscountType DiscountType { get; set; }

        public float Amount { get; set; }

        public List<int> SetsId { get; set; } = new();
    }
}
