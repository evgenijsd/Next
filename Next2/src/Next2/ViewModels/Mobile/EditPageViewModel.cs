using Prism.Navigation;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Next2.Models;
using Next2.Services.Order;
using System.Linq;
using Next2.Views;
using Prism.Services.Dialogs;
using Next2.Enums;
using Xamarin.CommunityToolkit.Helpers;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Contracts;
using Prism.Events;
using Next2.Helpers;

namespace Next2.ViewModels.Mobile
{
    public class EditPageViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;
        private readonly int _indexOfSeat;
        private readonly int _indexOfSet;

        public EditPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IPopupNavigation popupNavigation,
            IEventAggregator eventAggregator)
          : base(navigationService)
        {
            _eventAggregator = eventAggregator;

            _popupNavigation = popupNavigation;

            _orderService = orderService;

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);

            SelectedSet = _orderService.CurrentOrder.Seats[_indexOfSeat].SelectedItem;
        }

        #region -- Public properties --

        public SetBindableModel? SelectedSet { get; set; }

        private ICommand _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync);

        #endregion

        #region -- Private helpers --

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private async Task OnOpenModifyCommandAsync()
        {
            var navigationParameters = new NavigationParameters
            {
                 { Constants.Navigations.SELECTED_SET, SelectedSet },
            };
            await _navigationService.NavigateAsync(nameof(AddCommentPage), navigationParameters);
        }

        private async Task OnOpenRemoveCommandAsync()
        {
            var confirmDialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisSetWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = new Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseDeleteSetDialogCallbackAsync);

            await _popupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteSetDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isSetRemovingAccepted))
            {
                if (isSetRemovingAccepted)
                {
                    var result = await _orderService.DeleteSetFromCurrentSeat();

                    if (result.IsSuccess)
                    {
                        if (SelectedSet is not null)
                        {
                            _eventAggregator.GetEvent<RemoveSetEvent>().Publish(SelectedSet);
                        }

                        await _popupNavigation.PopAsync();
                        await _navigationService.GoBackAsync();
                    }
                }
                else
                {
                    await _popupNavigation.PopAsync();
                }
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }

        #endregion
    }
}
