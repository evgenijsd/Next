using Next2.Enums;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Linq;

namespace Next2.Extensions
{
    public static class OrderExtension
    {
        public static UpdateOrderCommand ToUpdateOrderCommand(this FullOrderBindableModel order)
        {
            return new UpdateOrderCommand()
            {
                Id = order.Id,
                Number = order.Number,
                OrderType = (EOrderType)order.OrderType,
                IsTab = order.IsTab,
                TableId = order.Table?.Id,
                IsSplitBySeats = order.IsSplitBySeats,
                Open = order.Open,
                Close = order.Close,
                OrderStatus = (EOrderStatus)order.OrderStatus,
                TaxCoefficient = order.TaxCoefficient,
                TotalPrice = order.TotalPrice,
                DiscountPrice = order.DiscountPrice,
                DiscountId = order.Discount?.Id,
                CouponId = order.Coupon?.Id,
                SubTotalPrice = order.SubTotalPrice,
                IsCashPayment = order.IsCashPayment,
                CustomerId = order.Customer?.Id,
                EmployeeId = order.EmployeeId ?? string.Empty,
                Seats = order.Seats.Select(row => row.ToIncomingSeatModel()),
            };
        }

        public static UpdateOrderCommand ToUpdateOrderCommand(this OrderModelDTO order)
        {
            Enum.TryParse(order.OrderType, out EOrderType type);

            return new()
            {
                Id = order.Id,
                Number = order.Number,
                OrderType = type,
                IsTab = order.IsTab,
                IsSplitBySeats = order.IsSplitBySeats,
                TableId = order.Table == null || order.Table?.Id == Guid.Empty
                    ? null
                    : order.Table?.Id,
                Open = order.Open,
                Close = order.Close,
                OrderStatus = order.OrderStatus,
                TaxCoefficient = order.TaxCoefficient,
                TotalPrice = order.TotalPrice,
                DiscountPrice = order.DiscountPrice,
                SubTotalPrice = order.SubTotalPrice,
                IsCashPayment = order.IsCashPayment,
                CouponId = order.Coupon?.Id,
                DiscountId = order.Discount == null || order.Discount?.Id == Guid.Empty
                    ? null
                    : order.Discount?.Id,
                CustomerId = order.Customer == null || order.Customer?.Id == Guid.Empty
                    ? null
                    : order.Customer?.Id,
                EmployeeId = order.EmployeeId ?? string.Empty,
                Seats = order.Seats.Select(row => row.ToIncomingSeatModel()),
            };
        }

        public static OrderModelDTO ToOrderModelDTO(this FullOrderBindableModel order)
        {
            return new()
            {
                Id = order.Id,
                Number = order.Number,
                IsTab = order.IsTab,
                Open = order.Open,
                Close = order.Close,
                IsCashPayment = order.IsCashPayment,
                IsSplitBySeats = order.IsSplitBySeats,
                Table = order.Table is not null
                    ? order.Table.Clone()
                    : new(),
                Customer = order.Customer is not null
                    ? order.Customer.ToSimpleCustomerModelDTO()
                    : new(),
                OrderStatus = (EOrderStatus)order.OrderStatus,
                OrderType = order.OrderType.ToString(),
                Discount = order.Discount,
                Coupon = order.Coupon,
                TaxCoefficient = order.TaxCoefficient,
                DiscountPrice = order.DiscountPrice,
                SubTotalPrice = order.SubTotalPrice,
                TotalPrice = order.TotalPrice,
                EmployeeId = order.EmployeeId,
                Seats = order.Seats is null
                ? Enumerable.Empty<SeatModelDTO>()
                : order.Seats.OrderBy(x => x.SeatNumber).Select(row => row.ToSeatModelDTO()),
            };
        }

        public static FullOrderBindableModel ToFullOrderBindableModel(this OrderModelDTO order)
        {
            var fullOrderBindableModel = new FullOrderBindableModel()
            {
                Id = order.Id,
                Number = order.Number,
                IsTab = order.IsTab,
                Open = order.Open,
                Close = order.Close,
                IsCashPayment = order.IsCashPayment,
                IsSplitBySeats = order.IsSplitBySeats,
                Table = order.Table is null
                    ? null
                    : order.Table.Clone(),
                Customer = order.Customer is null
                    ? null
                    : order.Customer.ToCustomerBindableModel(),
                OrderStatus = order.OrderStatus,
                OrderType = (EOrderType?)Enum.Parse(typeof(EOrderType), order.OrderType),
                Discount = order.Discount,
                Coupon = order.Coupon,
                TaxCoefficient = order.TaxCoefficient,
                DiscountPrice = order.DiscountPrice,
                SubTotalPrice = order.SubTotalPrice,
                TotalPrice = order.TotalPrice,
                PriceTax = order.SubTotalPrice is not null
                    ? (decimal)order.SubTotalPrice * order.TaxCoefficient
                    : new(),
                EmployeeId = order.EmployeeId,
                Seats = order.Seats is null
                    ? new()
                    : new(order.Seats.OrderBy(x => x.Number).Select(row => row.ToSeatBindableModel())),
            };

            foreach (var seat in fullOrderBindableModel.Seats)
            {
                foreach (var dish in seat.SelectedDishes)
                {
                    if (dish.SelectedProducts is not null)
                    {
                        foreach (var product in dish.SelectedProducts)
                        {
                            product.Price = СalculatePriceOfProportion(product.Product.DefaultPrice, dish.SelectedDishProportion.PriceRatio);
                        }
                    }
                }
            }

            return fullOrderBindableModel;
        }

        private static decimal СalculatePriceOfProportion(decimal price, decimal priceRatio)
        {
            return priceRatio == 1
                ? price
                : price * (1 + priceRatio);
        }
    }
}