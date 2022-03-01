using Next2.ENums;
using Next2.Interfaces;

namespace Next2.Models
{
    public struct Day
    {
        public string DayOfMonth { get; set; }

        public EDayState State { get; set; }
    }
}
