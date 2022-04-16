using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Next2.Models
{
    public class MemberBindableModel : BindableBase, IBaseModel, ITappable
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public DateTime MembershipStartTime { get; set; }

        public DateTime MembershipEndTime { get; set; }

        public ICommand? TapCommand { get; set; }
    }
}
