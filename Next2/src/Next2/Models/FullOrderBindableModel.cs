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
            int index = -1;

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
                var newSeat = new SeatBindableModel(seat);

                foreach (var set in seat.Sets)
                {
                    var newSet = new SetBindableModel(set);

                    foreach (var portion in set.Portions)
                    {
                        newSet.Portions.Add(new PortionModel(portion));
                    }

                    var tmpPortion = newSet.Portions.FirstOrDefault(row => row.Id == set.Portion.Id);
                    index = newSet.Portions.IndexOf(tmpPortion);

                    newSet.Portion = newSet.Portions[index];

                    foreach (var product in set.Products)
                    {
                        var newProduct = new ProductBindableModel(product);

                        foreach (var option in product.Options)
                        {
                            newProduct.Options.Add(new OptionModel(option));
                        }

                        foreach (var replacementProducts in product.ReplacementProducts)
                        {
                            newProduct.ReplacementProducts.Add(new ProductModel(replacementProducts));
                        }

                        if(product.SelectedOption is not null)
                        {
                            var tmpOption = newProduct.Options.FirstOrDefault(row => row.Id == product.SelectedOption.Id);
                            index = newProduct.Options.IndexOf(tmpOption);

                            newProduct.SelectedOption = newProduct.Options[index];
                        }

                        var tmpReplacementProduct = newProduct.ReplacementProducts.FirstOrDefault(row => row.Id == product.SelectedProduct.Id);

                        if (tmpReplacementProduct is not null)
                        {
                            index = newProduct.ReplacementProducts.IndexOf(tmpReplacementProduct);
                            newProduct.SelectedProduct = newProduct.ReplacementProducts[index];
                        }

                        newSet.Products.Add(newProduct);
                    }

                    newSeat.Sets.Add(newSet);
                }

                if(seat.SelectedItem is not null)
                {
                    var tmpSet = newSeat.Sets.FirstOrDefault(row => row.Id == seat.SelectedItem.Id);
                    index = newSeat.Sets.IndexOf(tmpSet);

                    newSeat.SelectedItem = newSeat.Sets[index];
                }

                Seats.Add(newSeat);
            }
        }

        public void UpdateTotalSum()
        {
            Total = 0;

            foreach (var seat in Seats)
            {
                foreach (var set in seat.Sets)
                {
                    set.Price = set.Portion.Price;
                    Total += set.Price;
                }
            }

            SubTotal = Total - Tax;
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