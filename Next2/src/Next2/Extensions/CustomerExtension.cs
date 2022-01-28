using Next2.Models;
using Next2.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Extensions
{
    public static class CustomerExtension
    {
        public static CustomersViewModel ToCustomersViewModel(this CustomerModel cm)
        {
            return new CustomersViewModel
            {
                Id = cm.Id,
                Name = cm.Name,
                Email = cm.Email,
                Rewards = cm.Rewards,
                Phone = cm.Phone,
                Points = cm.Points,
                GiftCardCount = cm.GiftCardCount,
                GiftCardTotal = cm.GiftCardTotal,
            };
        }

        public static CustomerModel ToCustomerModel(this CustomersViewModel cvm)
        {
            return new CustomerModel
            {
                Id = cvm.Id,
                Name = cvm.Name,
                Email = cvm.Email,
                Rewards = cvm.Rewards,
                Phone = cvm.Phone,
                Points = cvm.Points,
                GiftCardCount = cvm.GiftCardCount,
                GiftCardTotal = cvm.GiftCardTotal,
            };
        }
    }
}
