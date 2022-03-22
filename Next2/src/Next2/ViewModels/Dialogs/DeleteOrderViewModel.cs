using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class DeleteOrderViewModel : BindableBase
    {
        public DeleteOrderViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadPageData(param);
            RequestClose = requestClose;
            CancelCommand = new Command(() => RequestClose(null));
            DeleteOrderCommand = new Command(OnDeleteOrderCommand);
        }

        #region -- Public properties --

        public int OrderNumber { get; set; }

        public ObservableCollection<SeatModel> Seats { get; set; } = new ();

        public bool IsOrderDetailsDisplayed { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CancelCommand { get; }

        public ICommand DeleteOrderCommand { get; }

        private ICommand _showHideOrderDetailsCommand;
        public ICommand ShowHideOrderDetailsCommand => _showHideOrderDetailsCommand ??= new Command(OnShowHideOrderDetailsCommand);

        #endregion

        #region -- Private helpers --

        private void LoadPageData(IDialogParameters dialogParameters)
        {
            if (dialogParameters is not null
                && dialogParameters.TryGetValue(Constants.DialogParameterKeys.ORDER_NUMBER, out int orderNumber))
            {
                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.SEATS, out IEnumerable<SeatModel> seats))
                {
                    OrderNumber = orderNumber;

                    if (seats is not null)
                    {
                        Seats = new (seats);
                    }
                }
            }
        }

        private void OnShowHideOrderDetailsCommand()
        {
            IsOrderDetailsDisplayed = !IsOrderDetailsDisplayed;
        }

        private void OnDeleteOrderCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.ACCEPT, true } };

            RequestClose(dialogParameters);
        }

        #endregion
    }
}