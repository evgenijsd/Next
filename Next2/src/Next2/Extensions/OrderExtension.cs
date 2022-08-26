using Next2.Enums;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Extensions
{
    public static class OrderExtension
    {
        public static UpdateOrderCommand ToUpdateOrderCommand(this FullOrderBindableModel order)
        {
            var seats = order.Seats.Select(row => row.ToIncomingSeatModel());

            var updateOrderCommand = new UpdateOrderCommand()
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
                Seats = seats,
            };

            return updateOrderCommand;
        }

        public static UpdateOrderCommand ToUpdateOrderCommand(this OrderModelDTO order)
        {
            var seats = order.Seats.Select(row => row.ToIncomingSeatModel());

            Enum.TryParse(order.OrderType, out EOrderType type);

            UpdateOrderCommand command = new()
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
                Seats = seats,
            };

            return command;
        }

        public static OrderModelDTO ToOrderModelDTO(this FullOrderBindableModel order)
        {
            OrderModelDTO orderModelDTO = new()
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
            };

            List<SeatModelDTO> seatsModels = order.Seats is null
                ? new()
                : new(order.Seats.OrderBy(x => x.SeatNumber).Select(row => new SeatModelDTO()
                {
                    Number = row.SeatNumber,
                    SelectedDishes = row?.SelectedDishes.Select(row => new SelectedDishModelDTO()
                    {
                        Id = row.Id,
                        DishId = row.DishId,
                        Name = row.Name,
                        ImageSource = row.ImageSource,
                        TotalPrice = row.TotalPrice,
                        IsSplitted = row.IsSplitted,
                        SplitPrice = row.SplitPrice,
                        HoldTime = row.HoldTime,
                        DiscountPrice = row.DiscountPrice,
                        SelectedDishProportion = row.SelectedDishProportion?.Clone(),
                        SelectedProducts = row?.SelectedProducts.Select(row => row.ToSelectedProductModelDTO()),
                    }),
                }));

            orderModelDTO.Seats = seatsModels;

            return orderModelDTO;
        }

        public static FullOrderBindableModel ToFullOrderBindableModel(this OrderModelDTO order)
        {
            FullOrderBindableModel fullOrderBindableModel = new()
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
            };

            fullOrderBindableModel.Seats = order.Seats is null
                ? new()
                : new(order.Seats.OrderBy(x => x.Number).Select(row => new SeatBindableModel()
                {
                    SeatNumber = row.Number,
                    IsFirstSeat = row.Number == 1,
                    Checked = row.Number == 1,
                    SelectedDishes = new(row?.SelectedDishes.Select(row => row.ToDishBindableModel())),
                }));

            foreach (var seat in fullOrderBindableModel.Seats)
            {
                foreach (var dish in seat.SelectedDishes)
                {
                    if (dish.SelectedProducts is not null)
                    {
                        foreach (var product in dish.SelectedProducts)
                        {
                            var priceRatio = dish.SelectedDishProportion.PriceRatio;

                            product.Price = СalculatePriceOfProportion(product.Product.DefaultPrice, priceRatio);

                            if (product.AddedIngredients is not null)
                            {
                                foreach (var addedIngredient in product.AddedIngredients)
                                {
                                    addedIngredient.Price = СalculatePriceOfProportion(addedIngredient.Price, priceRatio);
                                }
                            }

                            if (product.ExcludedIngredients is not null)
                            {
                                foreach (var excludedIngredient in product.ExcludedIngredients)
                                {
                                    excludedIngredient.Price = СalculatePriceOfProportion(excludedIngredient.Price, priceRatio);
                                }
                            }
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