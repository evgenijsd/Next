using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.CustomersService
{
    public class CustomersService : ICustomersService
    {
        private readonly IMockService _mockService;

        public CustomersService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- ICustomersSerice implementation --

        public async Task<AOResult<int>> AddNewCustomerAsync(CustomerModel customer)
        {
            var result = new AOResult<int>();
            try
            {
                var response = await _mockService.AddAsync(customer);

                if (response != -1)
                {
                    result.SetSuccess(response);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddNewCustomerAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<CustomerModel>>> GetAllCustomersAsync(Func<CustomerModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CustomerModel>>();

            try
            {
                var customers = await _mockService.GetAllAsync<CustomerModel>();

                if (customers != null)
                {
                    result.SetSuccess(condition == null
                        ? customers
                        : customers.Where(condition));
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllCustomersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddGiftCardToCustomerAsync(CustomerModel customer, GiftCardModel giftCard)
        {
            var result = new AOResult();
            try
            {
                if (!customer.GiftCards.Contains(giftCard))
                {
                    giftCard.IsRegistered = true;
                    customer.GiftCards.Add(giftCard);
                    customer.GiftCardTotal = customer.GiftCards.Sum(row => row.GiftCardFounds);
                    customer.GiftCardCount = customer.GiftCards.Count();
                    customer.IsUpdatedCustomer = true;

                    if (customer.GiftCards.Contains(giftCard))
                    {
                        result.SetSuccess();
                    }
                    else
                    {
                        result.SetFailure();
                    }
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddGiftCardToCustomerAsync)}: exception", "The giftcard in-use", ex);
            }

            return result;
        }

        public async Task<AOResult> ActivateGiftCardAsync(GiftCardModel giftCard)
        {
            var result = new AOResult();
            try
            {
                var isGiftCardRemoved = await _mockService.RemoveAsync(giftCard);

                if (isGiftCardRemoved)
                {
                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(ActivateGiftCardAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<GiftCardModel>> IsGiftCardExistsAsync(int giftCardNumber)
        {
            var result = new AOResult<GiftCardModel>();
            try
            {
                var giftCard = await _mockService.FindAsync<GiftCardModel>(x => x.GiftCardNumber == giftCardNumber);

                if (giftCard is not null)
                {
                    result.SetSuccess(giftCard);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(IsGiftCardExistsAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        #endregion
    }
}
