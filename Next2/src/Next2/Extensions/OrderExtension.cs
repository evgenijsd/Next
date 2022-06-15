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
                    SelectedDishProportionId = x.SelectedDishProportion.Proportion.Id,
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

            UpdateOrderCommand command = new();

            command.Id = order.Id;
            command.Number = order.Number;
            command.OrderType = (EOrderType)order.OrderType;
            command.IsTab = order.IsTab;
            command.TableId = order?.Table.Id;
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
            command.Seats = seats;

            return command;
        }
    }
}
