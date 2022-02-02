using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class CustomersViewModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string? _phone;
        public string? Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private int _rewards;
        public int Rewards
        {
            get => _rewards;
            set => SetProperty(ref _rewards, value);
        }

        private int _giftCardCount;
        public int GiftCardCount
        {
            get => _giftCardCount;
            set => SetProperty(ref _giftCardCount, value);
        }

        private int _points;
        public int Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        private double _giftCardTotal;
        public double GiftCardTotal
        {
            get => _giftCardTotal;
            set => SetProperty(ref _giftCardTotal, value);
        }

        private ImageSource? _checkboxImage;
        public ImageSource? CheckboxImage
        {
            get => _checkboxImage;
            set => SetProperty(ref _checkboxImage, value);
        }

        private ICommand? _mobselectcommand;
        public ICommand? MobSelectCommand
        {
            get => _mobselectcommand;
            set => SetProperty(ref _mobselectcommand, value);
        }

        private ICommand? _tabselectcommand;
        public ICommand? TabSelectCommand
        {
            get => _tabselectcommand;
            set => SetProperty(ref _tabselectcommand, value);
        }
    }
}
