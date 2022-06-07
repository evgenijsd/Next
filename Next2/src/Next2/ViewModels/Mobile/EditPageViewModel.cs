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
using Xamarin.Forms;
using Next2.Views.Mobile;
using Next2.Services.Menu;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class EditPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private int _indexOfSeat;

        public EditPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IMenuService menuService)
          : base(navigationService)
        {
            _orderService = orderService;
            _menuService = menuService;
        }

        #region -- Public properties --

        public DishBindableModel? SelectedDish { get; set; }

        private ICommand _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);

            SelectedDish = _orderService.CurrentOrder.Seats[_indexOfSeat].SelectedItem;
        }

        #endregion

        #region -- Private helpers --

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private async Task OnOpenModifyCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(ModificationsPage));
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

            await PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteSetDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isDishRemovingAccepted))
            {
                if (isDishRemovingAccepted)
                {
                    var result = await _orderService.DeleteDishFromCurrentSeat();

                    if (result.IsSuccess)
                    {
                        await PopupNavigation.PopAsync();

                        var navigationParameters = new NavigationParameters
                        {
                            { nameof(Constants.Navigations.DELETE_SET), Constants.Navigations.DELETE_SET },
                        };
                        await _navigationService.GoBackAsync(navigationParameters);
                    }
                }
                else
                {
                    await PopupNavigation.PopAsync();
                }
            }
            else
            {
                await PopupNavigation.PopAsync();
            }
        }

        #endregion

    }
}
