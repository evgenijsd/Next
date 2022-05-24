using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Models
{
    public class CustomerBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? Birthday { get; set; }

        public int Rewards { get; set; }

        public int GiftCardsCount { get; set; }

        public int Points { get; set; }

        public decimal GiftCardTotal { get; set; }

        public List<GiftCardModel> GiftCards { get; set; } = new();

        public ImageSource CheckboxImage { get; set; }

        public ICommand SelectItemCommand { get; set; }

        public ICommand ShowInfoCommand { get; set; }
    }
}
