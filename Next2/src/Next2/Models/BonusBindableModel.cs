using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Models
{
    public class BonusBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Value { get; set; }

        public EBonusValueType Type { get; set; }

        public ICommand? TapCommand { get; set; }
    }
}
