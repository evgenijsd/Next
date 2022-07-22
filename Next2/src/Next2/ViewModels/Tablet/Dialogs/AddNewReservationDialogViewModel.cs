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
        }

        #region -- Public properties --

        public ObservableCollection<int> GuestsAmountList { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int SelectedAmountGuests { get; set; }

        public int SelectedTable { get; set; }

        public string Notes { get; set; } = string.Empty;

        public DateTime? SelectedDate { get; set; } = null;

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public bool IsPMTimeFormat { get; set; }

        public bool IsValidName { get; set; }

        public bool IsValidPhone { get; set; }

        public bool CanAddNewReservation => IsValidName && IsValidPhone && SelectedTable > 0 && SelectedDate is not null;

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        private ICommand _addNewReservationCommand;
        public ICommand AddNewReservationCommand => _addNewReservationCommand ??= new AsyncCommand(OnAddNewReservationCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
        }

        #endregion

        #region -- Private helpers --

        private async Task OnAddNewReservationCommandAsync()
        {
        }

        private void GenerateGuestsAmountList()
        {
            GuestsAmountList = new();

            for (int i = 0; i < 25; i++)
            {
                GuestsAmountList.Add(i);
            }
        }

        #endregion
    }
}
