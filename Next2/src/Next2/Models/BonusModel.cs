using Next2.ENums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class BonusModel : IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Value { get; set; }

        public EBonusValueType Type { get; set; }
    }
}
