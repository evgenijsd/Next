using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Models
{
    public class FullOrderBindableModel : BindableBase, IBaseModel
    {
        public FullOrderBindableModel()
        {
        }

        public FullOrderBindableModel(FullOrderBindableModel order)
        {
            int idx = -1;

            Id = order.Id;
            OrderNumber = order.OrderNumber;
            Table = order.Table;
            CustomerName = order.CustomerName;
            OrderStatus = order.OrderStatus;
            OrderType = order.OrderType;
            SubTotal = order.SubTotal;
            Tax = order.Tax;
            Total = order.Total;
            Seats = new();

            foreach (var seat in order.Seats)
            {
                var newSeat = new SeatBindableModel()
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    Checked = seat.Checked,
                    IsFirstSeat = seat.IsFirstSeat,
                    SetSelectionCommand = seat.SetSelectionCommand,
                    SeatSelectionCommand = seat.SeatSelectionCommand,
                    SeatDeleteCommand = seat.SeatDeleteCommand,
                    Sets = new(),
                };

                foreach (var set in seat.Sets)
                {
                    var newSet = new SetBindableModel()
                    {
                        Id = set.Id,
                        SubcategoryId = set.SubcategoryId,
                        Title = set.Title,
                        Price = set.Price,
                        ImagePath = set.ImagePath,
                        Portions = new(),
                        Products = new(),
                    };

                    foreach (var portion in set.Portions)
                    {
                        var newPortion = new PortionModel()
                        {
                            Id = portion.Id,
                            SetId = portion.SetId,
                            Title = portion.Title,
                            Price = portion.Price,
                        };

                        newSet.Portions.Add(newPortion);
                    }

                    var tmpPortion = newSet.Portions.FirstOrDefault(row => row.Id == set.Portion.Id);
                    idx = newSet.Portions.IndexOf(tmpPortion);

                    newSet.Portion = newSet.Portions[idx];

                    foreach (var product in set.Products)
                    {
                        var newProduct = new ProductBindableModel()
                        {
                            Id = product.Id,
                            Options = new(),
                            Title = product.Title,
                            ImagePath = product.ImagePath,
                            Price = product.Price,
                        };

                        foreach (var option in product.Options)
                        {
                            var newOption = new OptionModel()
                            {
                                Id = option.Id,
                                ProductId = option.ProductId,
                                Title = option.Title,
                            };

                            newProduct.Options.Add(newOption);
                        }

                        var tmpProduct = newProduct.Options.FirstOrDefault(row => row.Id == product.SelectedOption.Id);
                        idx = newProduct.Options.IndexOf(tmpProduct);

                        newProduct.SelectedOption = newProduct.Options[idx];

                        newSet.Products.Add(newProduct);
                    }

                    newSeat.Sets.Add(newSet);
                }

                var tmpSet = newSeat.Sets.FirstOrDefault(row => row.Id == seat.SelectedItem.Id);
                idx = newSeat.Sets.IndexOf(tmpSet);

                newSeat.SelectedItem = newSeat.Sets[idx];
                Seats.Add(newSeat);
            }
        }

        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public TableBindableModel Table { get; set; }

        public string? CustomerName { get; set; }

        public string OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public double SubTotal { get; set; }

        public double Tax { get; set; }

        public double Total { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }
    }
}