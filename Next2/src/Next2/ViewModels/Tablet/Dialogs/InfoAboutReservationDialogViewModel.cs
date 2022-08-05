using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class InfoAboutReservationDialogViewModel : BindableBase
    {
        public InfoAboutReservationDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            if (param.TryGetValue(Constants.DialogParameterKeys.MODEL, out ReservationModel reservation))
            {
                Reservation = reservation;
            }
        }

        #region -- Public properties --

        public ReservationModel Reservation { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _removeCommand;
        public ICommand RemoveCommand => _removeCommand ??= new AsyncCommand(OnRemoveCommandAsync, allowsMultipleExecutions: false);

        private ICommand _assignCommand;
        public ICommand AssignCommand => _assignCommand ??= new AsyncCommand(OnAssignCommandAsync, allowsMultipleExecutions: false);

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new AsyncCommand(OnCancelCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnRemoveCommandAsync()
        {
            var param = new DialogParameters()
            {
                { Constants.DialogParameterKeys.ACTION, Constants.DialogParameterKeys.REMOVE },
            };

            RequestClose(param);

            return Task.CompletedTask;
        }

        private Task OnAssignCommandAsync()
        {
            var param = new DialogParameters()
            {
                { Constants.DialogParameterKeys.ACTION, Constants.DialogParameterKeys.ASSIGN },
            };

            RequestClose(param);

            return Task.CompletedTask;
        }

        private Task OnCancelCommandAsync()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        #endregion
    }
}
