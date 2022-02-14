using Next2.ENums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class DayModel : IBaseModel
    {
        public string Day { get; set; }

        public EDayState State { get; set; }

        public int Id { get; set; }
    }
}
