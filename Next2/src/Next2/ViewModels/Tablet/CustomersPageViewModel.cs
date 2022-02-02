﻿using Next2;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.ViewModels;
using Next2.Views.Mobile.Dialogs;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace InterTwitter.ViewModels
{
    public class CustomersPageViewModel : BaseViewModel
    {
        private readonly ICustomersService _customersService;
        public CustomersPageViewModel(INavigationService navigationService, ICustomersService customersService)
            : base(navigationService)
        {
            _customersService = customersService;
        }

        #region -- Public Properties --

        private ObservableCollection<CustomersViewModel>? _customersList;

        public ObservableCollection<CustomersViewModel>? CustomersList
        {
            get => _customersList;
            set => SetProperty(ref _customersList, value);
        }

        private CustomerModel _customer;

        public CustomerModel Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        private CustomersViewModel _oldSelectedItem;
        private CustomersViewModel _selectedItem;

        public CustomersViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ICommand TabInfoButtonCommand => new AsyncCommand(OnTabInfoButtonCommand);
        public ICommand SortCommand => new AsyncCommand<string>(OnSortCommand);

        #endregion

        #region --Overrides--

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            await InitAsync();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
        }

        #endregion

        #region --Private Helpers--

        private async Task InitAsync()
        {
            var cl = await _customersService.GetAllCustomersAsync();
            if (cl.IsSuccess)
            {
                var list = cl.Result;
                var listvm = list.Select(x => x.ToCustomersViewModel());
                CustomersList = new ObservableCollection<CustomersViewModel>();
                foreach (var item in listvm)
                {
                    item.CheckboxImage = "ic_check_box_unhecked_24x24";
                    item.MobSelectCommand = new Command<object>(OnMobSelectButtonCommand);
                    item.TabSelectCommand = new Command<object>(OnTabSelectCommand);
                    CustomersList?.Add(item);
                }
            }
        }

        private void OnTabSelectCommand(object obj)
        {
            SelectedItem = obj as CustomersViewModel;

            if (_oldSelectedItem == null)
            {
                    _oldSelectedItem = SelectedItem;
                    SelectedItem.CheckboxImage = "ic_check_box_checked_primary_24x24";
            }
            else
            {
                if (SelectedItem == _oldSelectedItem)
                {
                    SelectedItem.CheckboxImage = "ic_check_box_unhecked_24x24";
                    SelectedItem = null;
                    _oldSelectedItem = null;
                }
                else
                {
                    var si = CustomersList.Where(x => x.Id == _oldSelectedItem.Id).FirstOrDefault();
                    si.CheckboxImage = "ic_check_box_unhecked_24x24";
                    SelectedItem.CheckboxImage = "ic_check_box_checked_primary_24x24";
                    _oldSelectedItem = SelectedItem;
                }
            }
        }

        private async Task OnTabInfoButtonCommand()
        {
            if (SelectedItem != null)
            {
                var param = new DialogParameters();
                param.Add(Constants.DialogParameterKeys.MODEL, SelectedItem);
                param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
                param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");
                await Rg.Plugins.Popup.Services
                    .PopupNavigation.Instance
                    .PushAsync(new CustomerInfoDialogTab(param, CloseDialogCallback));
            }
        }

        private async void OnMobSelectButtonCommand(object obj)
        {
            SelectedItem = obj as CustomersViewModel;
            var param = new DialogParameters();
            param.Add(Constants.DialogParameterKeys.MODEL, obj as CustomersViewModel);
            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");
            await Rg.Plugins.Popup.Services
                .PopupNavigation.Instance
                .PushAsync(new CustomerInfoDialogMob(param, CloseDialogCallback));
        }

        private async void CloseDialogCallback(IDialogParameters obj)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        private bool _isSortedAscending;
        private Task OnSortCommand(string param)
        {
            switch (param)
            {
                case "Name":
                    {
                        if (_isSortedAscending)
                        {
                            CustomersList = new ObservableCollection<CustomersViewModel>(CustomersList
                                .OrderByDescending(x => x.Name));
                            _isSortedAscending = false;
                        }
                        else
                        {
                            CustomersList = new ObservableCollection<CustomersViewModel>(CustomersList
                                .OrderBy(x => x.Name));
                            _isSortedAscending = true;
                        }

                        break;
                    }

                case "Points":
                    {
                        if (_isSortedAscending)
                        {
                            CustomersList = new ObservableCollection<CustomersViewModel>(CustomersList
                                .OrderByDescending(x => x.Points));
                            _isSortedAscending = false;
                        }
                        else
                        {
                            CustomersList = new ObservableCollection<CustomersViewModel>(CustomersList
                                .OrderBy(x => x.Points));
                            _isSortedAscending = true;
                        }

                        break;
                    }

                case "Phone":
                    {
                        if (_isSortedAscending)
                        {
                            CustomersList = new ObservableCollection<CustomersViewModel>(CustomersList
                                .OrderByDescending(x => x.Phone));
                            _isSortedAscending = false;
                        }
                        else
                        {
                            CustomersList = new ObservableCollection<CustomersViewModel>(CustomersList
                                .OrderBy(x => x.Phone));
                            _isSortedAscending = true;
                        }

                        break;
                    }
            }

            return Task.CompletedTask;
        }

        #endregion

    }
}
