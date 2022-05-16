﻿using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Models
{
    public class CustomerBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public System.Guid UuId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int Rewards { get; set; }

        public int GiftCardsCount { get; set; }

        public int Points { get; set; }

        public float GiftCardsTotalFund { get; set; }

        public List<GiftCardModel> GiftCards { get; set; } = new();

        public ImageSource CheckboxImage { get; set; }

        public ICommand SelectItemCommand { get; set; }

        public ICommand ShowInfoCommand { get; set; }
    }
}
