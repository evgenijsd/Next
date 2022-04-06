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
            Id = order.Id;
            OrderNumber = order.OrderNumber;
            Table = order.Table;
            Customer = order.Customer;
            CustomerName = order.CustomerName;
            OrderStatus = order.OrderStatus;
            OrderType = order.OrderType;
            SubTotal = order.SubTotal;
            Tax = order.Tax;
            Total = order.Total;
            Seats = new();

            foreach (var seat in order.Seats)
            {
                Seats.Add(new SeatBindableModel(seat));
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

                    foreach (var product in set.Products)
                    {
                        Total += product.IngredientsPrice;
                    }
                }
            }

            SubTotal = Total - Tax;
        }

        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public TableBindableModel Table { get; set; }

        public CustomerModel Customer { get; set; } = new();

        public string? CustomerName { get; set; }

        public string OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public double SubTotal { get; set; }

        public double Tax { get; set; }

        public double Total { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }

        public EOrderPaymentStatus? PaymentStatus;
    }
}