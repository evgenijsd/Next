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

        public static FullOrderBindableModel OrderDTOToFullOrderBindableModel(this OrderModelDTO order)
        {
            FullOrderBindableModel fullOrderBindableModel = new()
            {
                Id = order.Id,
                Number = order.Number,
                IsTab = order.IsTab,
                Open = order.Open,
                Close = order.Close,
                IsCashPayment = order.IsCashPayment,
                Table = order.Table is not null
                    ? new()
                    {
                         Id = order.Table.Id,
                         Number = order.Table.Number,
                         SeatNumbers = order.Table.SeatNumbers,
                    }
                    : null,
                Customer = order.Customer is not null
                    ? new()
                    {
                        Id = order.Customer.Id,
                        FullName = order.Customer?.FullName,
                        Phone = order.Customer?.Phone,
                    }
                    : null,
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
                Seats = new(order.Seats.Select(row => new SeatBindableModel()
                {
                    SeatNumber = row.Number,
                    IsFirstSeat = row.Number == 1,
                    Checked = row.Number == 1,
                    SelectedDishes = new(row.SelectedDishes.Select(row => new DishBindableModel()
                    {
                        Id = row.Id,
                        DishId = row.DishId,
                        SelectedDishProportionPrice = row.TotalPrice,
                        Name = row.Name,
                        ImageSource = row.ImageSource,
                        TotalPrice = row.TotalPrice,
                        DiscountPrice = row.DiscountPrice,
                        SelectedDishProportion = new()
                        {
                            Id = row.SelectedDishProportion.Id,
                            PriceRatio = row.SelectedDishProportion.PriceRatio,
                            Proportion = new()
                            {
                                Id = row.SelectedDishProportion.Proportion.Id,
                                Name = row.SelectedDishProportion.Proportion.Name,
                            },
                        },
                        SelectedProducts = new(row.SelectedProducts.Select(row => new ProductBindableModel()
                        {
                            Id = row.Product.Id,
                            Comment = new(row.Comment),
                            Product = new SimpleProductModelDTO()
                            {
                                Id = row.Product.Id,
                                DefaultPrice = row.Product.DefaultPrice,
                                Name = row.Product.Name,
                                ImageSource = row.Product.ImageSource,
                                Ingredients = row.Product.Ingredients.Select(row => new SimpleIngredientModelDTO()
                                {
                                    Id = row.Id,
                                    Name = row.Name,
                                    Price = row.Price,
                                    ImageSource = row.ImageSource,
                                    IngredientsCategory = row.IngredientsCategory,
                                }),
                                Options = row.Product.Options,
                            },
                            SelectedOptions = row.SelectedOptions.FirstOrDefault(),
                            SelectedIngredients = new(row.SelectedIngredients.Select(row => new SimpleIngredientModelDTO()
                            {
                                Id = row.Id,
                                Name = row.Name,
                                Price = row.Price,
                                ImageSource = row.ImageSource,
                                IngredientsCategory = row.IngredientsCategory,
                            })),
                            AddedIngredients = new(row.AddedIngredients.Select(row => new SimpleIngredientModelDTO()
                            {
                                Id = row.Id,
                                Name = row.Name,
                                Price = row.Price,
                                ImageSource = row.ImageSource,
                                IngredientsCategory = row.IngredientsCategory,
                            })),
                            ExcludedIngredients = new(row.ExcludedIngredients.Select(row => new SimpleIngredientModelDTO()
                            {
                                Id = row.Id,
                                Name = row.Name,
                                Price = row.Price,
                                ImageSource = row.ImageSource,
                                IngredientsCategory = row.IngredientsCategory,
                            })),
                        })),
                    })),
                })),
            };

            foreach (var seat in fullOrderBindableModel.Seats)
            {
                foreach (var dish in seat.SelectedDishes)
                {
                    foreach (var product in dish.SelectedProducts)
                    {
                        product.Price = dish.SelectedDishProportion.PriceRatio == 1
                            ? product.Product.DefaultPrice
                            : product.Product.DefaultPrice * (1 + dish.SelectedDishProportion.PriceRatio);
                    }
                }
            }

            return fullOrderBindableModel;
        }
    }
}
