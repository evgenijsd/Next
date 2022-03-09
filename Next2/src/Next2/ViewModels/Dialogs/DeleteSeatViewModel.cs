using Next2.Enums;
using Next2.Models;
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
            LoadPageData(param);
            RequestClose = requestClose;
            AcceptCommand = new Command(() =>
            {
                var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.SEAT, _seat } };

                if (IsDeletingItemsSelected)
                {
                    dialogParameters.Add(Constants.DialogParameterKeys.ACTION, EActionWhenDeletingSeat.DeleteSets);
                }
                else
                {
                    dialogParameters.Add(Constants.DialogParameterKeys.ACTION, EActionWhenDeletingSeat.RedirectSets);
                    dialogParameters.Add(Constants.DialogParameterKeys.SEAT_NUMBER, SelectedSeatListItem.SeatNumber);
                }

                RequestClose(dialogParameters);
            });

            DeclineCommand = new Command(() => RequestClose(null));
        }

        #region -- Public properties --

        public bool IsDeletingItemsSelected { get; set; }

        public ObservableCollection<SeatListItemBindableModel> SeatsListItems { get; set; } = new ();

        public SeatListItemBindableModel SelectedSeatListItem { get; set; } = new ();

        public Action<IDialogParameters> RequestClose;

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        private ICommand _selectDeletingItemsCommand;
        public ICommand SelectDeletingItemsCommand => _selectDeletingItemsCommand ??= new Command(OnSelectDeletingItemsCommand);

        #endregion

        #region -- Private helpers --

        private void LoadPageData(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.SEAT, out SeatBindableModel currentSeat)
                && param.TryGetValue(Constants.DialogParameterKeys.SEAT_NUMBERS, out IEnumerable<int> seatNumbers))
            {
                _seat = currentSeat;

                if (seatNumbers.Count() > 1)
                {
                    var seatListItems = seatNumbers
                    .Where(x => x != currentSeat.SeatNumber)
                    .Select(x => new SeatListItemBindableModel { SeatNumber = x });

                    SeatsListItems = new(seatListItems);
                    SelectedSeatListItem = SeatsListItems.FirstOrDefault();
                }
                else
                {
                    IsDeletingItemsSelected = true;
                    SelectedSeatListItem.SeatNumber = currentSeat.SeatNumber;
                }
            }
        }

        private void OnSelectDeletingItemsCommand()
        {
            if (SeatsListItems.Any())
            {
                IsDeletingItemsSelected = !IsDeletingItemsSelected;
            }
        }

        #endregion
    }
}