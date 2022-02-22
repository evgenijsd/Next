﻿using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Models
{
    public class CustomerBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int Rewards { get; set; }

        public int GiftCardCount { get; set; }

        public int Points { get; set; }

        public double GiftCardTotal { get; set; }

        public ImageSource CheckboxImage { get; set; }

        public ICommand SelectItemCommand { get; set; }

        public ICommand ShowInfoCommand { get; set; }
    }
}