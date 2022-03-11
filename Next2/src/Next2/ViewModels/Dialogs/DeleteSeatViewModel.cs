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
        private SeatBindableModel _removalSeat;

        public DeleteSeatViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadPageData(param);
            RequestClose = requestClose;

            AcceptCommand = new Command(OnAcceptCommand);
            DeclineCommand = new Command(() => RequestClose(null));
        }

        #region -- Public properties --

        public ObservableCollection<SeatListItemBindableModel> SeatsListItems { get; set; } = new ();

        public SeatListItemBindableModel SelectedSeatListItem { get; set; } = new ();

        public bool IsDeletingSetsSelected { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        private ICommand _selectDeletingSetsCommand;
        public ICommand SelectDeletingSetsCommand => _selectDeletingSetsCommand ??= new Command(OnSelectDeletingSetsCommand);

        #endregion

        #region -- Private helpers --

        private void LoadPageData(IDialogParameters param)
        {
            if (param is not null
                && param.TryGetValue(Constants.DialogParameterKeys.REMOVAL_SEAT, out SeatBindableModel removalSeat)
                && param.TryGetValue(Constants.DialogParameterKeys.SEAT_NUMBERS_OF_CURRENT_ORDER, out IEnumerable<int> seatNumbersOfCurrentOrder))
            {
                _removalSeat = removalSeat;

                if (seatNumbersOfCurrentOrder.Count() > 1)
                {
                    var seatListItems = seatNumbersOfCurrentOrder
                        .Where(x => x != removalSeat.SeatNumber)
                        .Select(x => new SeatListItemBindableModel { SeatNumber = x });

                    SeatsListItems = new (seatListItems);
                    SelectedSeatListItem = SeatsListItems.FirstOrDefault();
                }
                else
                {
                    IsDeletingSetsSelected = true;
                    SelectedSeatListItem.SeatNumber = removalSeat.SeatNumber;
                }
            }
        }

        private void OnSelectDeletingSetsCommand()
        {
            if (SeatsListItems.Any())
            {
                IsDeletingSetsSelected = !IsDeletingSetsSelected;
            }
        }

        private void OnAcceptCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.REMOVAL_SEAT, _removalSeat } };

            if (IsDeletingSetsSelected)
            {
                dialogParameters.Add(Constants.DialogParameterKeys.ACTION_ON_SETS, EActionOnSets.DeleteSets);
            }
            else
            {
                dialogParameters.Add(Constants.DialogParameterKeys.ACTION_ON_SETS, EActionOnSets.RedirectSets);
                dialogParameters.Add(Constants.DialogParameterKeys.DESTINATION_SEAT_NUMBER, SelectedSeatListItem.SeatNumber);
            }

            RequestClose(dialogParameters);
        }

        #endregion
    }
}