using Next2.Enums;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Services.Customers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AddNewReservationDialogViewModel : BindableBase
    {
        private readonly ICustomersService _customersService;

        public AddNewReservationDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            ICustomersService customersService)
        {
            _customersService = customersService;

            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));

            GuestsAmountList = new()
            {
                new()
                {
                    TableNumber = 55,
                },
                new()
                {
                    TableNumber = 5455,
                },
                new()
                {
                    TableNumber = 5785,
                },
                new()
                {
                    TableNumber = 1255,
                },
                new()
                {
                    TableNumber = 33355,
                },
            };
        }

        #region -- Public properties --

        public ObservableCollection<TableBindableModel> GuestsAmountList { get; set; }

        public bool IsExpandGuestsAmount { get; set; }

        public int CurrentItem { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? SelectedDate { get; set; } = null;

        public bool IsValidName { get; set; }

        public bool IsValidPhone { get; set; }

        public bool IsValidEmail { get; set; }

        public bool CanAddNewCustomer => IsValidName && IsValidPhone && IsValidEmail && SelectedDate is not null;

        public bool IsEntriesEnabled { get; set; } = true;

        public EClientAdditionStep Step { get; set; } = EClientAdditionStep.Info;

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        private ICommand _goToStepCommand;
        public ICommand GoToStepCommand => _goToStepCommand ??= new AsyncCommand<EClientAdditionStep>(OnGoToStepCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        private ICommand _expandGuestsAmountCommand;
        public ICommand ExpandGuestsAmountCommand => _expandGuestsAmountCommand ??= new AsyncCommand(OnExpandGuestsAmountCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        private Task OnExpandGuestsAmountCommandAsync()
        {
            IsExpandGuestsAmount = true;

            return Task.CompletedTask;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(Step))
            {
                IsEntriesEnabled = Step == EClientAdditionStep.Info;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnAddNewCustomerCommandAsync()
        {
            if (CanAddNewCustomer)
            {
                var newCustomer = new CustomerBindableModel()
                {
                    Email = Email,
                    FullName = Name,
                    Phone = Phone,
                    Birthday = SelectedDate,
                };

                var result = await _customersService.CreateCustomerAsync(newCustomer);

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
