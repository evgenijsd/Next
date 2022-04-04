using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Models
{
    public class RewardBindabledModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SetId { get; set; }

        public string SetTitle { get; set; } = string.Empty;

        public bool IsApplied { get; set; }

        public bool IsCanBeApplied { get; set; } = true;

        public ICommand? SelectCommand { get; set; }
    }
}
