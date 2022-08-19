using Next2.Enums;
using Next2.Helpers;
using Next2.Services.Notifications;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class TipsPageViewModel : BaseViewModel
    {
        private readonly INotificationsService _notificationsService;

        private TipItem? _noTipItem;

        public TipsPageViewModel(
            INotificationsService notificationsService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _notificationsService = notificationsService;
        }

        #region -- Public properties --

        public ObservableCollection<TipItem> TipDisplayItems { get; set; } = new();

        public TipItem? SelectedTipItem { get; set; }

        private ICommand? _tapTipItemCommand;
        public ICommand TapTipItemCommand => _tapTipItemCommand ??= new AsyncCommand<TipItem?>(OnTapTipItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectNoTipItemCommand;
        public ICommand SelectNoTipItemCommand => _selectNoTipItemCommand ??= new AsyncCommand(OnSelectNoTipItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var tipType = ETipType.NoTip;
            var tipPercent = 0m;

            if (parameters.TryGetValue(Constants.Navigations.TIP_TYPE, out ETipType paramTipType))
            {
                tipType = paramTipType;
            }

            if (parameters.TryGetValue(Constants.Navigations.TIP_PERCENT, out decimal paramTipPercent))
            {
                tipPercent = paramTipPercent;
            }

            if (parameters.TryGetValue(Constants.Navigations.TIP_ITEMS, out ObservableCollection<TipItem> tipItems))
            {
                foreach (var tip in tipItems)
                {
                    tip.TapCommand = TapTipItemCommand;
                    tip.IsSelected = false;

                    if (tip.TipType != ETipType.NoTip)
                    {
                        TipDisplayItems.Add(tip);

                        if (tip.TipType == tipType && tip.PercentTip == tipPercent)
                        {
                            SelectedTipItem = tip;
                            SelectedTipItem.IsSelected = true;
                        }
                    }
                    else
                    {
                        _noTipItem = tip;
                    }
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnTapTipItemCommandAsync(TipItem? selectedTipItem)
        {
            if (selectedTipItem is not null)
            {
                selectedTipItem.IsSelected = true;

                if (SelectedTipItem is not null && selectedTipItem != SelectedTipItem)
                {
                    SelectedTipItem.IsSelected = false;
                }

                SelectedTipItem = selectedTipItem;

                if (SelectedTipItem?.TipType == ETipType.Other)
                {
                    PopupPage popupPage = new Views.Mobile.Dialogs.TipValueDialog(TipViewDialogCallBack);

                    await PopupNavigation.PushAsync(popupPage);
                }
            }
        }

        private async void TipViewDialogCallBack(IDialogParameters parameters)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.TIP_VALUE_DIALOG, out decimal value) && SelectedTipItem is not null)
            {
                SelectedTipItem.Value = value;
                SelectedTipItem.Text = LocalizationResourceManager.Current["CurrencySign"] + $" {value:F2}";
            }
        }

        private Task OnSelectNoTipItemCommandAsync()
        {
            if (SelectedTipItem is not null)
            {
                SelectedTipItem.IsSelected = false;
                SelectedTipItem = null;
            }

            return Task.CompletedTask;
        }

        private Task OnGoBackCommandAsync()
        {
            var tipItem = SelectedTipItem is null
                ? _noTipItem
                : SelectedTipItem;

            var parameters = new NavigationParameters()
            {
                { Constants.Navigations.TIP_PERCENT, tipItem },
            };

            return _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}
