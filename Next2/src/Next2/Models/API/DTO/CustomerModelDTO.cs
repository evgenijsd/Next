using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class CustomerModelDTO : IBaseApiModel
    {
        public CustomerModelDTO()
        {
        }

        public CustomerModelDTO(CustomerModelDTO customerModel)
        {
            Id = customerModel.Id;
            FullName = customerModel.FullName;
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

        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public DateTime? Birthday { get; set; }

        public string? MembershipId { get; set; }

        public IEnumerable<string>? GiftCardsId { get; set; }

        #region -- Over Properties --

        public int Points { get; set; }

        public int Rewards { get; set; }

        public int GiftCardsCount { get; set; }

        public decimal GiftCardsTotalFund { get; set; }

        public List<GiftCardModel> GiftCards { get; set; } = new();

        public bool IsNotRegistratedCustomer { get; set; }

        public bool IsUpdatedCustomer { get; set; }

        #endregion
    }
}
