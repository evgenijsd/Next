using Next2.Enums;
using Next2.Models.Bindables;
using Next2.Services.Menu;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class EditPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private int _indexOfSeat;
        private bool _isModifiedDish;

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
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.DISH_MODIFIED, out bool isModifiedDish))
            {
                _isModifiedDish = isModifiedDish;
            }

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);

            SelectedDish = new();

            SelectedDish = _orderService.CurrentOrder.Seats[_indexOfSeat].SelectedItem;
        }

        #endregion

        #region -- Private helpers --

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnOpenModifyCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(ModificationsPage));
        }

        private Task OnOpenRemoveCommandAsync()
        {
            var confirmDialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisDishWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = new Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseDeleteDishDialogCallbackAsync);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteDishDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isDishRemovingAccepted))
            {
                if (isDishRemovingAccepted)
                {
                    var result = await _orderService.DeleteDishFromCurrentSeatAsync();
                    await _orderService.UpdateCurrentOrderAsync();

                    if (result.IsSuccess)
                    {
                        await CloseAllPopupAsync();

                        var navigationParameters = new NavigationParameters
                        {
                            { nameof(Constants.Navigations.DELETE_DISH), Constants.Navigations.DELETE_DISH },
                        };
                        await _navigationService.GoBackAsync(navigationParameters);
                    }
                }
                else
                {
                    await CloseAllPopupAsync();
                }
            }
            else
            {
                await CloseAllPopupAsync();
            }
        }

        private Task OnGoBackCommandAsync()
        {
            var navigationParam = new NavigationParameters();

            if (_isModifiedDish)
            {
                navigationParam.Add(Constants.Navigations.DISH_MODIFIED, _isModifiedDish);
            }

            return _navigationService.GoBackAsync(navigationParam);
        }

        #endregion
    }
}
