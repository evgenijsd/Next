using Next2.Models;
using Next2.Models.API.DTO;

namespace Next2.Extensions
{
    public static class CustomerExtension
    {
        public static SimpleCustomerModelDTO ToSimpleCustomerModelDTO(this CustomerBindableModel customer)
        {
            return new()
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Phone = customer.Phone,
            };
        }

        public static CustomerBindableModel ToCustomerBindableModel(this SimpleCustomerModelDTO customer)
        {
            return new()
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Phone = customer.Phone,
            };
        }
    }
}
