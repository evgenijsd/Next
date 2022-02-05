using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class CustomerViewModel : BindableBase
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