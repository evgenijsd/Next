using Next2.Enums;
using Next2.Helpers;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class TipsPageViewModel : BaseViewModel
    {
        private TipItem _tipItem;

        public TipsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public ObservableCollection<TipItem> TipDisplayItems { get; set; } = new();

        public TipItem SelectedTipItem { get; set; }

        private ICommand _tapTipItemCommand;
        public ICommand TapTipItemCommand => _tapTipItemCommand = new AsyncCommand<object>(OnTapTipItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand = new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var tipType = ETipType.NoTip;
            var tipValue = 0m;

            if (parameters.TryGetValue(Constants.Navigations.TIP_TYPE, out ETipType paramTipType))
            {
                tipType = paramTipType;
            }

            if (parameters.TryGetValue(Constants.Navigations.TIP_VALUE, out decimal paramTipValue))
            {
                tipValue = paramTipValue;
            }

            if (parameters.TryGetValue(Constants.Navigations.TIP_ITEMS, out ObservableCollection<TipItem> tipItems))
            {
                foreach (var item in tipItems)
                {
                    item.TapCommand = TapTipItemCommand;

                    if (item.TipType != ETipType.NoTip)
                    {
                        TipDisplayItems.Add(item);
                    }

                    if (item.TipType == tipType && item.Value == tipValue)
                    {
                        _tipItem = item;
                    }
                }

                SelectedTipItem = _tipItem;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnTapTipItemCommandAsync(object? sender)
        {
            if (sender is TipItem item && item.TipType == ETipType.Other)
            {
                PopupPage popupPage = new Views.Mobile.Dialogs.TipValueDialog(TipViewDialogCallBack);

                await PopupNavigation.PushAsync(popupPage);
            }
            else if (sender is ETipType eTip && eTip == ETipType.NoTip)
            {
                SelectedTipItem = _tipItem;
            }
        }

        private async void TipViewDialogCallBack(IDialogParameters parameters)
        {
            await PopupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.TIP_VALUE_DIALOG, out decimal value))
            {
                SelectedTipItem.Value = value;
                SelectedTipItem.Text = LocalizationResourceManager.Current["CurrencySign"] + $" {value:F2}";
            }
        }

        private Task OnGoBackCommandAsync()
        {
            var parameters = new NavigationParameters()
            {
                { Constants.Navigations.TIP_VALUE, SelectedTipItem },
            };
            return _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}
