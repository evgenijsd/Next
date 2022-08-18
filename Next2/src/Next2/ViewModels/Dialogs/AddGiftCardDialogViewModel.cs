using Next2.Services.Customers;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Dialogs
{
    public class AddGiftCardDialogViewModel : BindableBase
    {
        private readonly ICustomersService _customersService;

        private IEnumerable<Guid> _listGiftCardIDs;

        public AddGiftCardDialogViewModel(
            IEnumerable<Guid> giftCardsId,
            ICustomersService customersService,
            Action<IDialogParameters> requestClose)
        {
            _listGiftCardIDs = giftCardsId;
            _customersService = customersService;
            RequestClose = requestClose;
        }

        #region -- Public properties --

        public string InputGiftCardNumber { get; set; } = string.Empty;

        public bool IsGiftCardNotExists { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand? _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new AsyncCommand(OnCloseCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _addGiftСardCommand;
        public ICommand AddGiftСardCommand => _addGiftСardCommand ??= new AsyncCommand(OnAddGiftCardCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnCloseCommandAsync()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        private async Task OnAddGiftCardCommandAsync()
        {
            var dialogParameters = new DialogParameters() { { Constants.DialogParameterKeys.GIFT_CARD_ADDED, true } };

            var giftCardModel = await _customersService.GetGiftCardByNumberAsync(InputGiftCardNumber);

            if (giftCardModel.IsSuccess && !_listGiftCardIDs.Contains(giftCardModel.Result.Id))
            {
                dialogParameters.Add(Constants.DialogParameterKeys.GIFT_GARD, giftCardModel.Result);

                IsGiftCardNotExists = false;

                RequestClose(dialogParameters);
            }
            else
            {
                IsGiftCardNotExists = true;
            }
        }

        #endregion
    }
}
