using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class RewardBindabledModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SetId { get; set; }

        public string SetTitle { get; set; } = string.Empty;
    }
}
