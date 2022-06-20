using Next2.Enums;
using Next2.Models.API.DTO;
using Next2.Models.API.Commands;
using Next2.Models.Bindables;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Next2.Extensions
{
    public static class OrderExtension
    {
        public static UpdateOrderCommand ToUpdateOrderCommand(this FullOrderBindableModel order)
        {
            var seats = order.Seats.Select(x => new IncomingSeatModel()
            {
                Number = x.SeatNumber,
                SelectedDishes = x.SelectedDishes.Select(x => new IncomingSelectedDishModel()
                {
                    TotalPrice = x.TotalPrice,
                    DiscountPrice = x.DiscountPrice,
                    DishId = x.DishId,
                    SelectedDishProportionId = x.SelectedDishProportion.Id,
                    SelectedProducts = x.SelectedProducts.Select(x => new IncomingSelectedProductModel()
                    {
                        ProductId = x.Product.Id,
                        AddedIngredientsId = x.AddedIngredients?.Select(x => x.Id),
                        SelectedIngredientsId = x.SelectedIngredients?.Select(x => x.Id),
                        ExcludedIngredientsId = x.ExcludedIngredients?.Select(x => x.Id),
                        Comment = x?.Comment,
                        SelectedOptionsId = new Guid[1] { x.SelectedOptions.Id },
                    }),
                }),
            });

            UpdateOrderCommand updateOrderCommand = new()
            {
                Id = order.Id,
                Number = order.Number,
                OrderType = (EOrderType)order.OrderType,
                IsTab = order.IsTab,
                TableId = order?.Table.Id,
                Open = order.Open,
                Close = order?.Close,
                OrderStatus = (EOrderStatus)order?.OrderStatus,
                TaxCoefficient = order.TaxCoefficient,
                TotalPrice = order.TotalPrice,
                DiscountPrice = order?.DiscountPrice,
                DiscountId = order?.Discount?.Id,
                CouponId = order?.Coupon?.Id,
                SubTotalPrice = order?.SubTotalPrice,
                IsCashPayment = order.IsCashPayment,
                CustomerId = order?.Customer?.Id,
                EmployeeId = order?.EmployeeId,
                Seats = seats,
            };

            return updateOrderCommand;
        }

        public static UpdateOrderCommand ToUpdateOrderCommand(this OrderModelDTO order)
        {
            var seats = order.Seats.Select(x => new IncomingSeatModel()
            {
                Number = x.Number,
                SelectedDishes = x.SelectedDishes.Select(x => new IncomingSelectedDishModel()
                {
                    TotalPrice = x.TotalPrice,
                    DiscountPrice = x.DiscountPrice,
                    DishId = x.DishId,
                    SelectedDishProportionId = x.SelectedDishProportion.Id,
                    SelectedProducts = x.SelectedProducts.Select(x => new IncomingSelectedProductModel()
                    {
                        Comment = x?.Comment,
                        ProductId = x.Product.Id,
                        SelectedOptionsId = x?.SelectedOptions.Select(x => x.Id).ToArray(),
                        SelectedIngredientsId = x.SelectedIngredients?.Select(x => x.Id),
                        AddedIngredientsId = x.AddedIngredients?.Select(x => x.Id),
                        ExcludedIngredientsId = x.ExcludedIngredients?.Select(x => x.Id),
                    }),
                }),
            });

            UpdateOrderCommand command = new()
            {
                Id = order.Id,
                Number = order.Number,
                OrderType = (EOrderType)Enum.Parse(typeof(EOrderType), order.OrderType),
                IsTab = order.IsTab,
                TableId = order?.Table?.Id,
                Open = order.Open,
                Close = order?.Close,
                OrderStatus = (EOrderStatus)order?.OrderStatus,
                TaxCoefficient = order.TaxCoefficient,
                TotalPrice = order.TotalPrice,
                DiscountPrice = order?.DiscountPrice,
                SubTotalPrice = order?.SubTotalPrice,
                IsCashPayment = order.IsCashPayment,
                CouponId = order?.Coupon?.Id,
                DiscountId = order?.Discount?.Id,
                CustomerId = order?.Customer?.Id,
                EmployeeId = order?.EmployeeId,
                Seats = seats,
            };

            return command;
        }

        public static UpdateOrderCommand ToUpdateOrderCommand(this OrderModelDTO order)
        {
            UpdateOrderCommand command = new();
            Enum.TryParse(order.OrderType, out EOrderType type);

            command.Id = order.Id;
            command.Number = order.Number;
            command.OrderType = type;
            command.IsTab = order.IsTab;
            command.TableId = order?.Table?.Id;
            command.Open = order.Open;
            command.Close = order?.Close;
            command.OrderStatus = (EOrderStatus)order?.OrderStatus;
            command.TaxCoefficient = order.TaxCoefficient;
            command.TotalPrice = order.TotalPrice;
            command.DiscountPrice = order?.DiscountPrice;
            command.DiscountId = order?.Discount?.Id;
            command.CouponId = order?.Coupon?.Id;
            command.SubTotalPrice = order?.SubTotalPrice;
            command.IsCashPayment = order.IsCashPayment;
            command.CustomerId = order?.Customer?.Id;
            command.EmployeeId = order?.EmployeeId;
            command.Seats = order?.Seats?.Select(x => new IncomingSeatModel()
            {
                Number = x.Number,
                SelectedDishes = x.SelectedDishes?.Select(x => new IncomingSelectedDishModel()
                {
                    DiscountPrice = x?.DiscountPrice,
                    DishId = x.DishId,
                    SelectedDishProportionId = x.SelectedDishProportion.Id,
                    TotalPrice = x.TotalPrice,
                    SelectedProducts = x?.SelectedProducts?.Select(x => new IncomingSelectedProductModel()
                    {
                        AddedIngredientsId = x?.AddedIngredients?.Select(x => x.Id),
                        Comment = x?.Comment,
                        ExcludedIngredientsId = x?.ExcludedIngredients?.Select(x => x.Id),
                        ProductId = x?.Product.Id,
                        SelectedIngredientsId = x?.SelectedIngredients?.Select(x => x.Id),
                        SelectedOptionsId = x?.SelectedOptions?.Select(x => x.Id),
                    }),
                }),
            });

            return command;
        }
    }
}
