using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Next2.Models.Api.DTO;

namespace Next2.Models
{
    public class CustomerBindableModel : BindableBase, IBaseApiModel
    {
        public CustomerBindableModel()
        {
        }

        public CustomerBindableModel(CustomerBindableModel customerModel)
        {
            Id = customerModel.Id;
            FullName = customerModel.FullName;
            Email = customerModel.Email;
            Phone = customerModel.Phone;
            Birthday = customerModel.Birthday;
            Points = customerModel.Points;
            Rewards = customerModel.Rewards;
            GiftCards = customerModel.GiftCards;
            IsNotRegistratedCustomer = customerModel.IsNotRegistratedCustomer;
            IsUpdatedCustomer = customerModel.IsUpdatedCustomer;
        }

        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? Birthday { get; set; }

        public string? MembershipId { get; set; }

        public IEnumerable<Guid>? GiftCardsId { get; set; }

        public int Rewards { get; set; }

        public int Points { get; set; }

        public double GiftCardsTotalFund => GiftCards.Sum(x => x.TotalBalance);

        public List<GiftCardModelDTO> GiftCards { get; set; } = new();

        public bool IsNotRegistratedCustomer { get; set; }

        public bool IsUpdatedCustomer { get; set; }

        public ImageSource CheckboxImage { get; set; }

        public ICommand SelectItemCommand { get; set; }

        public ICommand ShowInfoCommand { get; set; }
    }
}
