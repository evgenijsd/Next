using Next2.Enums;
using Next2.Models.Api.DTO;
using Next2.Models.API.Commands;
using Next2.Models.Bindables;
using System;
using System.Linq;

namespace Next2.Helpers.Extensions
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
                    selectedProducts = x.SelectedProducts.Select(x => new IncomingSelectedProductModel()
                    {
                        ProductId = x.Product.Id,
                        AddedIngredientsId = x.AddedIngredients.Select(x => x.Id),
                        SelectedIngredientsId = x.SelectedIngredients.Select(x => x.Id),
                        ExcludedIngredientsId = x.ExcludedIngredients.Select(x => x.Id),
                        Comment = x.Comment,
                        SelectedOptionsId = new Guid[1] { x.SelectedOptions.Id },
                    }),
                }),
            });

            UpdateOrderCommand command = new()
            {
                Id = order.Id,
                Number = order.Number,
                OrderType = (EOrderType)order.OrderType,
                IsTab = order.IsTab,
                TableId = order.Table.Id,
                Open = order.Open,
                Close = order.Close,
                OrderStatus = (EOrderStatus)order.OrderStatus,
                TaxCoefficient = order.TaxCoefficient,
                TotalPrice = order.TotalPrice,
                DiscountPrice = order.DiscountPrice,
                DiscountId = order?.Discount.Id,
                CouponId = order?.Coupon.Id,
                SubTotalPrice = order.SubTotalPrice,
                IsCashPayment = order.IsCashPayment,
                CustomerId = order?.Customer.Id,
                EmployeeId = order.EmployeeId,
                Seats = seats,
            };

            return command;
        }
    }
}
