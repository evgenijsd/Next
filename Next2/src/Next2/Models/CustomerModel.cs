using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class CustomerModel : IBaseModel
    {
        public CustomerModel()
        {
        }

        public CustomerModel(CustomerModel customerModel)
        {
            Id = customerModel.Id;
            Name = customerModel.Name;
            Email = customerModel.Email;
            Phone = customerModel.Phone;
            Birthday = customerModel.Birthday;
            Points = customerModel.Points;
            Rewards = customerModel.Rewards;
            GiftCards = customerModel.GiftCards;
            GiftCardsCount = customerModel.GiftCardsCount;
            GiftCardsTotalFund = customerModel.GiftCardsTotalFund;
            IsNotRegistratedCustomer = customerModel.IsNotRegistratedCustomer;
            IsUpdatedCustomer = customerModel.IsUpdatedCustomer;
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        public int Points { get; set; }

        public int Rewards { get; set; }

        public int GiftCardsCount { get; set; }

        public float GiftCardsTotalFund { get; set; }

        public List<GiftCardModel> GiftCards { get; set; } = new();

        public bool IsNotRegistratedCustomer { get; set; }

        public bool IsUpdatedCustomer { get; set; }
    }
}
