using Next2.Helpers.DTO.Customers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Next2.Helpers.Extensions
{
    public static class CustomerModelDTOConverter
    {
        public static CustomerModelDTO MergeWithDTOModel(this CustomerModel customerModel, CustomerModelDTO customerModelDTO = null)
        {
            if (customerModelDTO is null)
            {
                customerModelDTO = new CustomerModelDTO()
                {
                    Id = customerModel.UuId,
                    FullName = customerModel.Name,
                    Email = customerModel.Email,
                    Phone = customerModel.Phone.Trim('-'),
                    Birthday = customerModel?.Birthday?.ToString("s"),
                    GiftCardsId = customerModel.GiftCardsId ??= customerModel?.GiftCards.Select(x => x.UuId),
                    MembershipId = customerModel?.MembershipId,
                };
            }
            else
            {
                customerModelDTO.Id = customerModel.UuId;
                customerModelDTO.FullName = customerModel.Name;
                customerModelDTO.Email = customerModel.Email;
                customerModelDTO.Phone = customerModel.Phone.Trim('-');
                customerModelDTO.Birthday = customerModel?.Birthday?.ToString("s");
                customerModelDTO.GiftCardsId = customerModel.GiftCardsId ??= customerModel?.GiftCards.Select(x => x.UuId);
                customerModelDTO.MembershipId = customerModel?.MembershipId;
            }

            return customerModelDTO;
        }

        public static CustomerModel MergeWithUIModel(this CustomerModelDTO customerModelDTO, CustomerModel customerModel = null)
        {
            if (customerModel is null)
            {
                var phone = customerModelDTO.Phone;

                customerModel = new CustomerModel()
                {
                    UuId = customerModelDTO.Id,
                    Name = customerModelDTO.FullName,
                    Email = customerModelDTO.Email,
                    Phone = $"{phone.Substring(0, 3)}-{phone.Substring(3, 3)}-{phone.Substring(6)}",
                    Birthday = DateTime.Parse(customerModelDTO.Birthday),
                    GiftCardsId = customerModelDTO.GiftCardsId,
                    MembershipId = customerModelDTO?.MembershipId,
                    GiftCardsCount = customerModelDTO.GiftCardsId.Count(),
                };
            }
            else
            {
                var phone = customerModelDTO.Phone;

                customerModel.UuId = customerModelDTO.Id;
                customerModel.Name = customerModelDTO.FullName;
                customerModel.Email = customerModelDTO.Email;
                customerModel.Phone = $"{phone.Substring(0, 3)}-{phone.Substring(3, 3)}-{phone.Substring(6)}";
                customerModel.Birthday = DateTime.Parse(customerModelDTO.Birthday);
                customerModel.GiftCardsId = customerModelDTO.GiftCardsId;
                customerModel.MembershipId = customerModelDTO?.MembershipId;
                customerModel.GiftCardsCount = customerModelDTO.GiftCardsId.Count();
            }

            return customerModel;
        }
    }
}
