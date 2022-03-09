using Next2.Enums;
using Next2.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class DeleteSeatViewModel : BindableBase
    {
        private SeatBindableModel _seat;

        public DeleteSeatViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadDataFromParameters(param);
            RequestClose = requestClose;
            AcceptCommand = new Command(() =>
            {
                var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.SOURCE_SEAT, _seat } };

                if (IsDeletingItemsSelected)
                {
                    dialogParameters.Add(Constants.DialogParameterKeys.ACTION, EActionWhenDeletingSeat.DeleteSets);
                }
                else
                {
                    dialogParameters.Add(Constants.DialogParameterKeys.ACTION, EActionWhenDeletingSeat.RedirectSets);
                    dialogParameters.Add(Constants.DialogParameterKeys.DESTINATION_SEAT_NUMBER, SelectedSeat.SeatNumber);
                }

                RequestClose(dialogParameters);
            });

            DeclineCommand = new DelegateCommand(() => RequestClose(null));
        }

        #region -- Public properties --

        public bool IsDeletingItemsSelected { get; set; }

        public ObservableCollection<SeatListItemBindableModel> Seats { get; set; } = new ();

        public SeatListItemBindableModel SelectedSeat { get; set; } = new ();

        public Action<IDialogParameters> RequestClose;

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        private ICommand _selectDeletingItemsCommand;
        public ICommand SelectDeletingItemsCommand => _selectDeletingItemsCommand ??= new Command(OnSelectDeletingItemsCommand);

        #endregion

        #region -- Private helpers --

        private void LoadDataFromParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.SOURCE_SEAT, out SeatBindableModel seat))
            {
                IEnumerable<int> otherSeatNumbers = Enumerable.Range(1, Constants.TABLE_SEATS_NUMBER)
                    .Where(x => x != seat.SeatNumber);

                var seats = otherSeatNumbers.Select(x => new SeatListItemBindableModel { SeatNumber = x });
                Seats = new (seats);

                SelectedSeat = Seats.FirstOrDefault();
                _seat = seat;
            }
        }

        private void OnSelectDeletingItemsCommand()
        {
            IsDeletingItemsSelected = !IsDeletingItemsSelected;
        }

        #endregion
    }
}