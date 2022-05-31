﻿using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Models
{
    public class FullOrderBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public bool IsTab { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new();

        public SimpleCustomerModelDTO Customer { get; set; } = new();

        public EOrderStatus? OrderStatus { get; set; }

        public EOrderType? OrderType { get; set; }

        public SimpleDiscountModelDTO Discount { get; set; } = new();

        public SimpleCouponModelDTO Coupon { get; set; } = new();

        public decimal TaxCoefficient { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal PriceTax { get; set; }

        public decimal TotalPrice { get; set; }

        public string? EmployeeId { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public void UpdateTotalSum()
        {
            SubTotalPrice = 0;

            foreach (var seat in Seats)
            {
                foreach (var dish in seat.SelectedDishes)
                {
                    foreach (var product in dish.SelectedProducts)
                    {
                        var ingredientsPrice = product.AddedIngredients is not null ? product.AddedIngredients.Sum(row => row.Price) : 0;
                        ingredientsPrice += product.ExcludedIngredients is not null ? product.ExcludedIngredients.Sum(row => row.Price) : 0;
                        var productPrice = product.Product.DefaultPrice;
                        dish.TotalPrice = dish.SelectedDishProportion.PriceRatio == 1 ? ingredientsPrice + productPrice : (ingredientsPrice + productPrice) * (1 + dish.SelectedDishProportion.PriceRatio);
                    }

                    SubTotalPrice += dish.TotalPrice;
                }
            }

            PriceTax = (decimal)SubTotalPrice * TaxCoefficient;
            TotalPrice = (decimal)SubTotalPrice + PriceTax;
        }
    }
}