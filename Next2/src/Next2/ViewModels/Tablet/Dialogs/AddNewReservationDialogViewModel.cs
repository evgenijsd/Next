using Next2.Services.Customers;
using Next2.Views.Tablet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AddNewReservationDialogViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        public AddNewReservationDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));

            GenerateCollection(GuestsAmount, 25);
            GenerateCollection(Tables, 10);

            SelectedDate = DateTime.Now;

            Hours = SelectedDate.ToString("hh");
            Minute = SelectedDate.ToString("mm");
            TimeType = SelectedDate.ToString("tt");

            ChangeSelectedTime();
        }

        #region -- Public properties --

        public string Name { get; set; }

        public string Phone { get; set; }

        public int SelectedAmountGuests { get; set; }

        public ObservableCollection<int> GuestsAmount { get; set; } = new();

        public int SelectedTable { get; set; }

        public ObservableCollection<int> Tables { get; set; } = new();

        public string Notes { get; set; } = string.Empty;

        public DateTime SelectedTime { get; set; } = new();

        public DateTime SelectedDate { get; set; }

        public DateTime MinimumSelectableDate { get; set; } = DateTime.Now;

        public string Hours { get; set; }

        public string Minute { get; set; }

        public string TimeType { get; set; } = string.Empty;

        public bool IsValidName { get; set; }

        public bool IsValidPhone { get; set; }

        public bool CanAddNewReservation { get; set; }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        private ICommand _changeTimeFormatCommand;
        public ICommand ChangeTimeFormatCommand => _changeTimeFormatCommand ??= new AsyncCommand<string>(OnChangeTimeFormatCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openPageInputCommentCommand;
        public ICommand GoInputCommentCommand => _openPageInputCommentCommand ??= new AsyncCommand(OnGoInputCommentCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName
                is nameof(IsValidName)
                or nameof(IsValidPhone)
                or nameof(SelectedAmountGuests))
            {
                ChangeCanAddNewReservation();
            }
            else if (args.PropertyName
                is nameof(SelectedDate)
                or nameof(Hours)
                or nameof(Minute)
                or nameof(TimeType))
            {
                ChangeSelectedTime();
                ChangeCanAddNewReservation();
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoInputCommentCommandAsync()
        {
            var param = new DialogParameters();

            var popupPage = new Views.Tablet.Dialogs.InputDialog(param, AddNewReservationDialogCallBack);

            await PopupNavigation.PushAsync(popupPage);
        }

        private void AddNewReservationDialogCallBack(IDialogParameters param)
        {
            if (PopupNavigation.PopupStack.Count > 0)
            {
                PopupNavigation.PopAsync();
            }
        }

        private async Task OnChangeTimeFormatCommandAsync(string state)
        {
            TimeType = state;
        }

        private void GenerateCollection(ObservableCollection<int> collection, int count)
        {
            for (int i = 1; i < count; i++)
            {
                collection.Add(i);
            }
        }

        private void ChangeSelectedTime()
        {
            var date = SelectedDate.ToString("MM/dd/yyyy");

            var dateTime = $"{date} {Hours}:{Minute} {TimeType}";

            SelectedTime = DateTime.Parse(dateTime);
        }

        private void ChangeCanAddNewReservation()
        {
            CanAddNewReservation = IsValidName && IsValidPhone && SelectedAmountGuests > 0 && SelectedTime > DateTime.Now;
        }

        #endregion
    }
}
