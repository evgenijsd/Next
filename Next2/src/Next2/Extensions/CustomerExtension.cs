using Next2.Models;

namespace Next2.Extensions
{
    public static class CustomerExtension
    {
        public static CustomerBindableModel ToCustomerBindableModel(this CustomerModel customerModel)
        {
            return new CustomerBindableModel
            {
                Id = customerModel.Id,
                Name = customerModel.Name,
                Email = customerModel.Email,
                Rewards = customerModel.Rewards,
                Phone = customerModel.Phone,
                Points = customerModel.Points,
                GiftCardCount = customerModel.GiftCardCount,
                GiftCardTotal = customerModel.GiftCardTotal,
            };
        }

        public static CustomerModel ToCustomerModel(this CustomerBindableModel customerBindableModel)
        {
            return new CustomerModel
            {
                Id = customerBindableModel.Id,
                Name = customerBindableModel.Name,
                Email = customerBindableModel.Email,
                Rewards = customerBindableModel.Rewards,
                Phone = customerBindableModel.Phone,
                Points = customerBindableModel.Points,
                GiftCardCount = customerBindableModel.GiftCardCount,
                GiftCardTotal = customerBindableModel.GiftCardTotal,
            };
        }
    }
}
