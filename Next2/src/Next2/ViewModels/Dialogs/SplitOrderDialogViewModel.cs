using Next2.Enums;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class SplitOrderDialogViewModel : BindableBase
    {
        private decimal _maxValue = 100;
        private List<int[]> _splitGroupList = new();

        public SplitOrderDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            LoadData(param);
        }

        #region -- Public Properties --

        public OrderModelDTO Order { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public SeatBindableModel SelectedSeat { get; set; }

        public List<object> SelectedSeats { get; set; } = new();

        public ESplitOrderConditions Condition { get; set; }

        public decimal SplitValue { get; set; }

        public decimal SplitTotal => Seats == null || Seats.Count() == 0
                ? 0
                : Seats.Sum(x => x.SelectedItem.TotalPrice);

        public bool IsSplitAvailable { get; set; }

        public string HeaderText { get; set; }

        public string Step { get; set; } = "First";

        public Action<IDialogParameters> RequestClose;

        private ICommand _selectValueCommand;
        public ICommand SelectValueCommand => _selectValueCommand ??= new AsyncCommand<object>(OnSelectValueCommandAsync, allowsMultipleExecutions: false);

        private ICommand _incrementCommand;
        public ICommand IncrementCommand => _incrementCommand ??= new AsyncCommand(OnIncrementCommandAsync, allowsMultipleExecutions: false);

        private ICommand _decrementCommand;
        public ICommand DecrementCommand => _decrementCommand ??= new AsyncCommand(OnDecrementCommandAsync, allowsMultipleExecutions: false);

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(() => RequestClose(new DialogParameters { }));

        private ICommand _splitCommand;
        public ICommand SplitCommand => _splitCommand ??= new AsyncCommand(OnSplitCommandAsync, allowsMultipleExecutions: false);

        private ICommand _selectCommand;
        public ICommand SelectCommand => _selectCommand ??= new Command<object>(OnSelectCommand);

        private ICommand _nextCommand;
        public ICommand NextCommand => _nextCommand ??= new AsyncCommand(OnNextCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedSeat):
                    {
                        if (Condition == ESplitOrderConditions.ByPercents)
                        {
                            SplitValue = decimal.Round(SelectedSeat.SelectedItem.TotalPrice / SelectedDish.TotalPrice * 100);
                        }
                        else if (Condition == ESplitOrderConditions.ByDollar)
                        {
                            SplitValue = SelectedSeat.SelectedItem.TotalPrice;
                        }

                        break;
                    }

                case nameof(SplitValue):
                    {
                        if (Condition == ESplitOrderConditions.ByPercents)
                        {
                            CalculateByPercentage();
                        }
                        else if (Condition == ESplitOrderConditions.ByDollar)
                        {
                            CalculateByDollar();
                        }

                        IsSplitAvailable = SplitTotal > 0;

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
            var isAllParamExist = param.TryGetValue(Constants.DialogParameterKeys.SEATS, out ObservableCollection<SeatBindableModel> seats)
                & param.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish)
                & param.TryGetValue(Constants.DialogParameterKeys.DESCRIPTION, out ESplitOrderConditions condition);

            if (isAllParamExist)
            {
                Condition = condition;
                SelectedDish = selectedDish;
                IEnumerable<SeatBindableModel> newSeats;

                if (condition == ESplitOrderConditions.BySeats)
                {
                    HeaderText = Strings.SplitBySeats;

                    newSeats = seats.Select(x => new SeatBindableModel()
                    {
                        SeatNumber = x.SeatNumber,
                        SelectedItem = new DishBindableModel() { TotalPrice = x.SelectedDishes.Sum(x => x.TotalPrice), },
                    });
                }
                else
                {
                    HeaderText = condition == ESplitOrderConditions.ByPercents
                        ? Strings.SplitByPercentage
                        : Strings.SplitByDollar;

                    newSeats = seats.Where(x => x.Checked is false).Select(x => new SeatBindableModel()
                    {
                        SeatNumber = x.SeatNumber,
                        SelectedItem = new DishBindableModel() { TotalPrice = 0, },
                    });
                }

                Seats = new ObservableCollection<SeatBindableModel>(newSeats);
            }
        }

        private Task OnSplitCommandAsync()
        {
            if (IsSplitAvailable)
            {
                if (Condition == ESplitOrderConditions.BySeats)
                {
                    IsSplitAvailable = false;

                    var seats = SelectedSeats.Select(x => x as SeatBindableModel);
                    var seatNumbers = seats.Select(x => x.SeatNumber).ToArray();

                    _splitGroupList.Add(seatNumbers);

                    foreach (var seat in seats)
                    {
                        Seats.Remove(seat);
                    }

                    SelectedSeats = new();

                    if (Seats.Count == 0)
                    {
                        DialogParameters param = new()
                        {
                            { Constants.DialogParameterKeys.SPLIT_GROUPS, _splitGroupList },
                        };

                        RequestClose.Invoke(param);
                    }
                }
                else
                {
                    SelectedDish.TotalPrice -= SplitTotal;

                    DialogParameters param = new()
                    {
                        { Constants.DialogParameterKeys.SEATS, Seats.Where(x => x.SelectedItem.TotalPrice > 0).ToList() },
                    };

                    RequestClose.Invoke(param);
                }
            }

            return Task.CompletedTask;
        }

        private void CalculateByPercentage()
        {
            if (SelectedSeat is not null)
            {
                var price = SplitValue * SelectedDish.TotalPrice / 100;
                var otherSeats = Seats.Where(x => x.SeatNumber != SelectedSeat.SeatNumber && x.SelectedItem.TotalPrice != 0);
                var splitTotalPrise = price + otherSeats.Sum(x => x.SelectedItem.TotalPrice);

                if (splitTotalPrise <= SelectedDish.TotalPrice)
                {
                    SelectedSeat.SelectedItem.TotalPrice = price;

                    RaisePropertyChanged(nameof(SplitTotal));
                }
            }
        }

        private void CalculateByDollar()
        {
            if (SelectedSeat is not null)
            {
                var otherSeats = Seats.Where(x => x.SeatNumber != SelectedSeat.SeatNumber && x.SelectedItem.TotalPrice != 0);
                var splitTotalPrise = SplitValue + otherSeats.Sum(x => x.SelectedItem.TotalPrice);

                if (splitTotalPrise <= SelectedDish.TotalPrice)
                {
                    SelectedSeat.SelectedItem.TotalPrice = SplitValue;
                }
            }
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

        private void OnSelectCommand(object sender)
        {
            var collectionView = sender as CollectionView;
            IsSplitAvailable = collectionView.SelectedItems.Count > 0;
        }

        private Task OnNextCommand()
        {
            Step = "Second";
            return Task.CompletedTask;
        }

        #endregion

    }
}
