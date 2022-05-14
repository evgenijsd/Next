﻿using Next2.Helpers.DTO.Customers;
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
                    Phone = customerModel.Phone,
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
                customerModelDTO.Phone = customerModel.Phone;
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
                customerModel = new CustomerModel()
                {
                    UuId = customerModelDTO.Id,
                    Name = customerModelDTO.FullName,
                    Email = customerModelDTO.Email,
                    Phone = customerModelDTO.Phone,
                    Birthday = DateTime.Parse(customerModelDTO.Birthday),
                    GiftCardsId = customerModelDTO.GiftCardsId,
                    MembershipId = customerModelDTO?.MembershipId,
                    GiftCardsCount = customerModelDTO.GiftCardsId.Count(),
                };
            }
            else
            {
                customerModel.UuId = customerModelDTO.Id;
                customerModel.Name = customerModelDTO.FullName;
                customerModel.Email = customerModelDTO.Email;
                customerModel.Phone = customerModelDTO.Phone;
                customerModel.Birthday = DateTime.Parse(customerModelDTO.Birthday);
                customerModel.GiftCardsId = customerModelDTO.GiftCardsId;
                customerModel.MembershipId = customerModelDTO?.MembershipId;
                customerModel.GiftCardsCount = customerModelDTO.GiftCardsId.Count();
            }

            return customerModel;
        }
    }
}
