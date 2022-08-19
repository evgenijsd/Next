using Next2.Models.Bindables;
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
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            if (parameters.TryGetValue(Constants.DialogParameterKeys.MODEL, out ReservationBindableModel reservation))
            {
                Reservation = reservation;
            }
        }

        #region -- Public properties --

        public ReservationBindableModel Reservation { get; set; } = new();

        public Action<IDialogParameters> RequestClose;

        private ICommand? _removeCommand;
        public ICommand RemoveCommand => _removeCommand ??= new AsyncCommand(OnRemoveCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _assignCommand;
        public ICommand AssignCommand => _assignCommand ??= new AsyncCommand(OnAssignCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new AsyncCommand(OnCancelCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnRemoveCommandAsync()
        {
            var parameters = new DialogParameters()
            {
                { Constants.DialogParameterKeys.ACTION, Constants.DialogParameterKeys.REMOVE },
            };

            RequestClose(parameters);

            return Task.CompletedTask;
        }

        private Task OnAssignCommandAsync()
        {
            var parameters = new DialogParameters()
            {
                { Constants.DialogParameterKeys.ACTION, Constants.DialogParameterKeys.ASSIGN },
            };

            RequestClose(parameters);

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
