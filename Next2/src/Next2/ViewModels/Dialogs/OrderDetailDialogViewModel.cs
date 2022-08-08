using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class OrderDetailDialogViewModel : BindableBase
    {
        public OrderDetailDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadPageData(param);

            RequestClose = requestClose;

            CancelCommand = new Command(() => RequestClose(new DialogParameters()));
            DeleteOrderCommand = new Command(OnDeleteOrderCommand);
        }

        #region -- Public properties --

        public int OrderNumber { get; set; }

        public string Title { get; set; }

        public string CancellationText { get; set; }

        public string ConfirmationText { get; set; }

        public Color OkButtonBackgroundColor { get; set; }

        public Color OkButtonTextColor { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public bool IsOrderDetailsDisplayed { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CancelCommand { get; }

        public ICommand DeleteOrderCommand { get; }

        private ICommand? _showHideOrderDetailsCommand;
        public ICommand ShowHideOrderDetailsCommand => _showHideOrderDetailsCommand ??= new Command(OnShowHideOrderDetailsCommand);

        #endregion

        #region -- Private helpers --

        private void LoadPageData(IDialogParameters dialogParameters)
        {
            if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.ORDER_NUMBER, out int orderNumber))
            {
                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.SEATS, out IEnumerable<SeatBindableModel> seats))
                {
                    OrderNumber = orderNumber;

                    if (seats is not null)
                    {
                        Seats = new(seats);
                    }
                }

                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.TITLE, out string title))
                {
                    Title = title;
                }

                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, out string cancellationText))
                {
                    CancellationText = cancellationText;
                }

                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT, out string confirmationText))
                {
                    ConfirmationText = confirmationText;
                }

                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_BACKGROUND, out Color okButtonBackGroundcolor))
                {
                    OkButtonBackgroundColor = okButtonBackGroundcolor;
                }

                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT_COLOR, out Color okButtonTextColor))
                {
                    OkButtonTextColor = okButtonTextColor;
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