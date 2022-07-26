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
            Hours = "05";
            Minutes = "32";
        }

        #region -- Public properties --

        public string Name { get; set; }

        public string Phone { get; set; }

        public int SelectedAmountGuests { get; set; }

        public ObservableCollection<int> GuestsAmount { get; set; } = new();

        public int SelectedTable { get; set; }

        public ObservableCollection<int> Tables { get; set; } = new();

        public string Notes { get; set; } = string.Empty;

        public DateTime? SelectedDate { get; set; }

        public DateTime MinimumSelectableDate { get; set; } = DateTime.Now;

        public string Hours { get; set; }

        public string Minutes { get; set; }

        public bool IsPMTimeFormat { get; set; }

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

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName
                is nameof(IsValidName)
                or nameof(IsValidPhone)
                or nameof(SelectedAmountGuests)
                or nameof(SelectedDate))
            {
                CanAddNewReservation = IsValidName && IsValidPhone && SelectedAmountGuests > 0 && SelectedDate is not null;
            }
        }

        #endregion

        #region -- Private helpers --

        private void InputNotesHandler(ReservationsViewModel sender, string inputNotes)
        {
            Notes = inputNotes;

            MessagingCenter.Unsubscribe<ReservationsViewModel>(this, Constants.Navigations.INPUT_NOTES);
        }

        private INavigation Navigation => App.Current.MainPage.Navigation;

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnGoInputCommentCommandAsync()
        {
            var param = new DialogParameters();

            var popupPage = new Views.Tablet.Dialogs.InputDialog(param, AddNewReservationDialogCallBack)
            {
                Animation = new Rg.Plugins.Popup.Animations.MoveAnimation(MoveAnimationOptions.Bottom, MoveAnimationOptions.Top),
            };

            await PopupNavigation.PushAsync(popupPage);

            //MessagingCenter.Subscribe<ReservationsViewModel, string>(this, Constants.Navigations.INPUT_NOTES, InputNotesHandler);

            //var navigationParameters = new NavigationParameters()
            //{
            //    { Constants.Navigations.INPUT_NOTES, Notes },
            //    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["CommentForReservation"] },
            //};

            //await _navigationService.NavigateAsync(nameof(SearchPage), navigationParameters, true);
        }

        private void AddNewReservationDialogCallBack(IDialogParameters obj)
        {
        }

        private async Task OnChangeTimeFormatCommandAsync(string state)
        {
            if (bool.TryParse(state, out bool result))
            {
                IsPMTimeFormat = result;
            }
        }

        private void GenerateCollection(ObservableCollection<int> collection, int count)
        {
            for (int i = 1; i < count; i++)
            {
                collection.Add(i);
            }
        }

        #endregion
    }
}
