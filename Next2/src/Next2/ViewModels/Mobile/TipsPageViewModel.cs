using Next2.Helpers;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class TipsPageViewModel : BaseViewModel
    {
        private readonly IPopupNavigation _popupNavigation;

        private ICommand _tapTipsItemCommand;

        public TipsPageViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _tapTipsItemCommand = new AsyncCommand<TipItem>(OnTapTipsValuesCommandAsync, allowsMultipleExecutions: false);
            _popupNavigation = popupNavigation;
        }

        #region -- Public properties --

        public ObservableCollection<TipItem> TipValueItems { get; set; } = new();

        public TipItem SelectedTipItem { get; set; } = new();

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand = new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.TIP_ITEMS, out ObservableCollection<TipItem> tipItems))
            {
                TipValueItems = tipItems;

                foreach (var item in TipValueItems)
                {
                    item.TapCommand = _tapTipsItemCommand;
                }

                SelectedTipItem = TipValueItems[0];
            }
        }

        #endregion

        #region -- Private methods --

        private async Task OnTapTipsValuesCommandAsync(TipItem? item)
        {
            if (item?.PercentTip == 1)
            {
                PopupPage popupPage = new Views.Mobile.Dialogs.TipValueDialog(TipViewDialogCallBack);

                await _popupNavigation.PushAsync(popupPage);
            }
        }

        private async void TipViewDialogCallBack(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.TIP_VALUE, out float value))
            {
                SelectedTipItem.Value = value;
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            var parameters = new NavigationParameters()
            {
                { Constants.Navigations.TIP_VALUE, SelectedTipItem },
            };
            await _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}
