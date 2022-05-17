using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Next2.Models
{
    public class MemberBindableModel : BindableBase, IBaseApiModel, ITappable
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public SimpleCustomerModelDTO Customer { get; set; } = new();

        public ICommand? TapCommand { get; set; }
    }
}
