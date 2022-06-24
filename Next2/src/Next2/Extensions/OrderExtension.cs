using Next2.Enums;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
                TableId = order?.Table?.Id,
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

        public static FullOrderBindableModel ToFullOrderBindableModel(this OrderModelDTO order)
        {
            FullOrderBindableModel bindableOrder = null;

            try
            {
                var seats = order.Seats.Select(x => new SeatBindableModel()
                {
                    SeatNumber = x.Number,
                    SelectedDishes = new(x.SelectedDishes.Select(x => new DishBindableModel()
                    {
                        Id = x.Id,
                        ImageSource = x.ImageSource,
                        Name = x.Name,
                        TotalPrice = x.TotalPrice,
                        DiscountPrice = x.DiscountPrice,
                        DishId = x.DishId,
                        SelectedDishProportion = new()
                        {
                            Id = x.SelectedDishProportion.Id,
                            PriceRatio = x.SelectedDishProportion.PriceRatio,
                            Proportion = new()
                            {
                                Id = x.SelectedDishProportion.Id,
                                Name = "Not defined",
                            },
                        },
                        SelectedProducts = new(x.SelectedProducts.Select(x => new ProductBindableModel()
                        {
                            Comment = x?.Comment,
                            Product = x.Product,
                            SelectedOptions = x?.SelectedOptions.FirstOrDefault(),
                            SelectedIngredients = new(x?.SelectedIngredients),
                            AddedIngredients = new(x.AddedIngredients),
                            ExcludedIngredients = new(x.ExcludedIngredients),
                        })),
                    })),
                });

                bindableOrder = new()
                {
                    Id = order.Id,
                    Number = order.Number,
                    OrderType = (EOrderType)Enum.Parse(typeof(EOrderType), order.OrderType),
                    IsTab = order.IsTab,
                    Table = order?.Table,
                    Open = order.Open,
                    Close = order?.Close,
                    OrderStatus = (EOrderStatus)order?.OrderStatus,
                    TaxCoefficient = order.TaxCoefficient,
                    TotalPrice = order.TotalPrice,
                    DiscountPrice = order?.DiscountPrice,
                    SubTotalPrice = order?.SubTotalPrice,
                    PriceTax = (decimal)(order.SubTotalPrice * order.TaxCoefficient),
                    IsCashPayment = order.IsCashPayment,
                    Coupon = order?.Coupon,
                    Discount = order?.Discount,
                    Customer = new()
                    {
                        Id = order.Customer is null
                            ? Guid.Empty
                            : order.Customer.Id,
                        FullName = order.Customer?.FullName,
                    },
                    EmployeeId = order?.EmployeeId,
                    Seats = new(seats),
                };
            }
            catch (Exception ex)
            {
            }

            return bindableOrder;
        }
    }
}