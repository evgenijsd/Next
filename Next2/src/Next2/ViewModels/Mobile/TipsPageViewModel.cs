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
        private readonly IPopupNavigation _popupNavigation;

        private TipItem _noTipItem;

        public TipsPageViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
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
            if (parameters.TryGetValue(Constants.Navigations.TIP_ITEMS, out ObservableCollection<TipItem> tipItems))
            {
                foreach (var item in tipItems)
                {
                    item.TapCommand = TapTipItemCommand;
                    if (item.TipType != ETipType.NoTip)
                    {
                        TipDisplayItems.Add(item);
                    }
                    else
                    {
                        _noTipItem = item;
                    }
                }

                SelectedTipItem = _noTipItem;
            }
        }

        #endregion

        #region -- Private methods --

        private async Task OnTapTipItemCommandAsync(object? sender)
        {
            if (sender is TipItem item && item.TipType == ETipType.Other)
            {
                PopupPage popupPage = new Views.Mobile.Dialogs.TipValueDialog(TipViewDialogCallBack);

                await _popupNavigation.PushAsync(popupPage);
            }
            else if (sender is ETipType eTip && eTip == ETipType.NoTip)
            {
                SelectedTipItem = _noTipItem;
            }
        }

        private async void TipViewDialogCallBack(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.TIP_VALUE_DIALOG, out float value))
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
