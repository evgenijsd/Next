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
        public DeleteSeatViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadDataFromParameters(param);
            RequestClose = requestClose;
            AcceptCommand = new Command(() =>
            {
                var dialogParameters = IsDeletingItemsSelected
                    ? new DialogParameters { { Constants.DialogParameterKeys.ACTION, EActionWhenDeletingSeat.DeleteSets } }
                    : new DialogParameters
                    {
                        { Constants.DialogParameterKeys.ACTION, EActionWhenDeletingSeat.RedirectSets },
                        { Constants.DialogParameterKeys.SEAT_NUMBER, SelectedSeat.SeatNumber },
                    };

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
            if (param.TryGetValue(Constants.DialogParameterKeys.SEAT_NUMBER, out int currentSeatNumber))
            {
                IEnumerable<int> otherSeatNumbers = Enumerable.Range(1, Constants.TABLE_SEATS_NUMBER)
                    .Where(x => x != currentSeatNumber);

                var seats = otherSeatNumbers.Select(x => new SeatListItemBindableModel { SeatNumber = x });
                Seats = new (seats);

                SelectedSeat = Seats.FirstOrDefault();
            }
        }

        private void OnSelectDeletingItemsCommand()
        {
            IsDeletingItemsSelected = !IsDeletingItemsSelected;
        }

        #endregion
    }
}