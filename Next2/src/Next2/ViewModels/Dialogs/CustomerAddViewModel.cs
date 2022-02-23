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
        private Color _acceptColorMob;
        private Color _acceptColorTab;
        public CustomerAddViewModel(DialogParameters param, Action<IDialogParameters> requestClose, ICustomersService customersService)
        {
            _customersService = customersService;

            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));

            SelectedDate = null;

            _acceptColorTab = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
            _acceptColorMob = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];

            DoneButtonOpacity = 0.32;
        }

        #region --Public Properties--

        private bool _accept => (WarningTextColor == _acceptColorTab || WarningTextColor == _acceptColorMob) && Email != null && Email != string.Empty && Name != null && Name != string.Empty && Phone != null && Phone != string.Empty && SelectedDate != null;

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public Color WarningTextColor { get; set; }

        public double DoneButtonOpacity { get; set; }

        private ICommand _doneCommand;
        public ICommand DoneCommand => _doneCommand ?? new AsyncCommand(AddNewCustomer);

        public DateTime? SelectedDate { get; set; }

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        public Action<IDialogParameters> RequestClose;

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (_accept)
            {
                DoneButtonOpacity = 1;
            }
            else
            {
                DoneButtonOpacity = 0.32;
            }
        }

        #region --Private Helpers--

        private async Task AddNewCustomer()
        {
            if (_accept)
            {
                var newCustomer = new CustomerModel() { Email = Email, Name = Name, Phone = Phone, Birthday = SelectedDate };
                var result = await _customersService.AddNewCustomer(newCustomer);

                if (result.IsSuccess)
                {
                    AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true }, { "Id", result.Result } }));
                }

                AcceptCommand.Execute();
            }
        }

        #endregion
    }
}
