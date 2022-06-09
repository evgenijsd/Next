using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
     public class SplitOrderDialogViewModel : BindableBase
    {
        private decimal _maxValue = 100;

        public SplitOrderDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            AcceptCommand = new Command(() => RequestClose(null));
            DeclineCommand = new Command(() => RequestClose(null));

            LoadData(param);
        }

        #region -- Public Properties --

        public ESplitOrderConditions Condition { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public SeatBindableModel SelectedSeat { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }

        public FullOrderBindableModel Order { get; set; }

        public decimal SplitValue { get; set; }

        public decimal SplitTotal => Seats == null || Seats.Count() == 0
                ? 0
                : Seats.Sum(x => x.SelectedItem.TotalPrice);

        public Action<IDialogParameters> RequestClose;

        private ICommand _selectValueCommand;
        public ICommand SelectValueCommand => _selectValueCommand ??= new AsyncCommand<object>(OnSelectValueCommandAsync, allowsMultipleExecutions: false);

        private ICommand _incrementCommand;
        public ICommand IncrementCommand => _incrementCommand ??= new AsyncCommand(OnIncrementCommandAsync, allowsMultipleExecutions: false);

        private ICommand _decrementCommand;
        public ICommand DecrementCommand => _decrementCommand ??= new AsyncCommand(OnDecrementCommandAsync, allowsMultipleExecutions: false);

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            switch (args.PropertyName)
            {
                case nameof(SelectedSeat):
                    {
                        SplitValue = decimal.Round(SelectedSeat.SelectedItem.TotalPrice / SelectedDish.TotalPrice * 100);
                        Calculate();
                        break;
                    }

                case nameof(SplitValue):
                    {
                        Calculate();
                        break;
                    }

                default:
                    break;
            }
        }

        #endregion

        #region -- Private Helpers --

        private void LoadData(IDialogParameters param)
        {
            var isAllParamExist = param.TryGetValue(Constants.DialogParameterKeys.MODEL, out FullOrderBindableModel order)
                & param.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish)
                & param.TryGetValue(Constants.DialogParameterKeys.DESCRIPTION, out ESplitOrderConditions condition);

            if (isAllParamExist)
            {
                Condition = condition;
                Order = order;
                SelectedDish = new DishBindableModel(selectedDish);

                var seats = Order.Seats.Where(x => x.Checked is false).Select(x => new SeatBindableModel()
                {
                    SeatNumber = x.SeatNumber,
                    Id = x.Id,
                    SeatSelectionCommand = new AsyncCommand(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false),
                    SelectedItem = new DishBindableModel(selectedDish) { TotalPrice = 0, },
                });

                Seats = new ObservableCollection<SeatBindableModel>(seats);
            }
        }

        private Task OnSeatSelectionCommandAsync()
        {
            throw new NotImplementedException();
        }

        private Task OnSelectValueCommandAsync(object? arg)
        {
            decimal.TryParse(arg as string, out decimal value);
            SplitValue = value;
            return Task.CompletedTask;
        }

        private Task OnIncrementCommandAsync()
        {
            if (SplitValue < _maxValue)
            {
                SplitValue++;
            }

            return Task.CompletedTask;
        }

        private Task OnDecrementCommandAsync()
        {
            if (SplitValue > 0)
            {
                SplitValue--;
            }

            return Task.CompletedTask;
        }

        private void Calculate()
        {
            if (SelectedSeat is not null)
            {
                var price = (SplitValue * SelectedDish.TotalPrice) / 100;
                var otherSeats = Seats.Where(x => x.SeatNumber != SelectedSeat.SeatNumber && x.SelectedItem.TotalPrice != 0);
                var splitTotalPrise = price + otherSeats.Sum(x => x.SelectedItem.TotalPrice);

                if (splitTotalPrise <= SelectedDish.TotalPrice)
                {
                    SelectedSeat.SelectedItem.TotalPrice = price;

                    RaisePropertyChanged(nameof(SplitTotal));
                }

                //else
                //{
                //    SelectedSeat.SelectedItem.TotalPrice = price;

                //    foreach (var seat in otherSeats)
                //    {
                //        seat.SelectedItem.TotalPrice = 0;
                //    }

                //    RaisePropertyChanged(nameof(SplitTotal));
                //}
            }
        }

        #endregion

    }
}
