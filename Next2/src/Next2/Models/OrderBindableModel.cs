using Next2.Enums;
using Prism.Mvvm;
using System;

namespace Next2.Models
{
    public class OrderBindableModel : BindableBase
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }

        public int OrderNumber { get; set; }

        private string? _name;
        public string? Name
        {
            get => _name;
            set { _name = string.IsNullOrWhiteSpace(value) ? CreateRandomCustomName() : value; }
        }

        public string? OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public BonusBindableModel Bonus { get; set; } = new();

        public string OrderNumberText { get; set; } = string.Empty;

        public double Total { get; set; }

        public double Tax { get; set; }

        #region -- Private helpers --

        private string CreateRandomCustomName()
        {
            string[] names = { "Bob", "Tom", "Sam" };

            string[] surnames = { "White", "Black", "Red" };

            Random random = new();

            string name = names[random.Next(2)];

            string surname = surnames[random.Next(3)];

            return name + " " + surname;
        }

        #endregion
    }
}
