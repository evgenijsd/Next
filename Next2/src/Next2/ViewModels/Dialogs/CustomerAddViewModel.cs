﻿using Next2.Enums;
using Next2.Models;
using Next2.Services.CustomersService;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class CustomerAddViewModel : BindableBase
    {
        private readonly ICustomersService _customersService;
        //private readonly Color _acceptanceColorOnMobile = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
        //private readonly Color _acceptanceColorOnTablet = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
        public CustomerAddViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            ICustomersService customersService)
        {
            _customersService = customersService;

            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
        }

        #region -- Public properties --

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? SelectedDate { get; set; } = null;

        public bool IsValidName { get; set; }

        public bool IsValidPhone { get; set; }

        public bool IsValidEmail { get; set; }

        public bool CanAddNewCustomer => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Email) &&
            IsValidName && IsValidPhone && IsValidEmail && SelectedDate is not null;

        public EClientAdditionStep Step { get; set; } = EClientAdditionStep.Info;

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        private ICommand _goToStepCommand;
        public ICommand GoToStepCommand => _goToStepCommand ??= new AsyncCommand<EClientAdditionStep>(OnGoToStepCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private async Task OnAddNewCustomerCommandAsync()
        {
            if (CanAddNewCustomer)
            {
                var newCustomer = new CustomerModel()
                {
                    Email = Email,
                    Name = Name,
                    Phone = Phone,
                    Birthday = SelectedDate,
                };

                var result = await _customersService.AddNewCustomerAsync(newCustomer);

                if (result.IsSuccess)
                {
                    var parameters = new DialogParameters()
                    {
                        { Constants.DialogParameterKeys.ACCEPT, true },
                        { Constants.DialogParameterKeys.CUSTOMER_ID, result.Result },
                    };

                    AcceptCommand = new DelegateCommand(() => RequestClose(parameters));
                }

                AcceptCommand.Execute();
            }
        }

        private Task OnGoToStepCommandAsync(EClientAdditionStep step)
        {
            Step = step;
            return Task.CompletedTask;
        }

        #endregion
    }
}
