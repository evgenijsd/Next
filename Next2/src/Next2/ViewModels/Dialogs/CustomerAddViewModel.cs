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
        private readonly Color _acceptanceColorOnMobile = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
        private readonly Color _acceptanceColorOnTablet = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];

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

        public double DoneButtonOpacity { get; set; } = 0.32;

        public Color WarningTextColor { get; set; }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync);

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName
                is nameof(SelectedDate)
                or nameof(Name)
                or nameof(Phone)
                or nameof(Email))
            {
                DoneButtonOpacity = CanAddNewCustomer
                    ? 1
                    : 0.32;
            }
        }

        #region -- Private helpers --

        private bool CanAddNewCustomer => (WarningTextColor == _acceptanceColorOnTablet || WarningTextColor == _acceptanceColorOnMobile)
            && SelectedDate is not null
            && !string.IsNullOrEmpty(Email)
            && !string.IsNullOrEmpty(Name)
            && !string.IsNullOrEmpty(Phone);

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
                        { Constants.DialogParameterKeys.ID, result.Result },
                    };

                    AcceptCommand = new DelegateCommand(() => RequestClose(parameters));
                }

                AcceptCommand.Execute();
            }
        }

        #endregion
    }
}
