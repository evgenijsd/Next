using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class DiscountModelDTO : SimpleDiscountModelDTO, IBaseApiModel
    {
        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
