using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Next2.Models
{
    public class MemberBindableModel : BindableBase, IBaseApiModel, ITappable
    {
        public Guid Id { get; set; }

        public string? CustomerName { get; set; }

        public string? Phone { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public Guid CustomerId { get; set; }

        public ICommand? TapCommand { get; set; }
    }
}
