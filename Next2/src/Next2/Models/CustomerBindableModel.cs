using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

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
            IsCustomerRegistrated = customerModel.IsCustomerRegistrated;
            IsUpdatedCustomer = customerModel.IsUpdatedCustomer;
        }

        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? Birthday { get; set; }

        public string? MembershipId { get; set; }

        public IEnumerable<Guid>? GiftCardsId
        {
            get => GiftCards.Select(x => x.Id);
            set => GiftCardsId = value;
        }

        public int Rewards { get; set; }

        public int Points { get; set; }

        public decimal GiftCardsTotalFund => GiftCards.Where(x => x.Expire.ToOADate() >= DateTime.Now.ToOADate()).Sum(x => x.TotalBalance);

        public List<GiftCardModelDTO> GiftCards { get; set; } = new();

        public bool IsCustomerRegistrated { get; set; }

        public bool IsUpdatedCustomer { get; set; }

        public ImageSource CheckboxImage { get; set; }

        public ICommand SelectItemCommand { get; set; }

        public ICommand ShowInfoCommand { get; set; }
    }
}
