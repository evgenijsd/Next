using Next2.Enums;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class SplitOrderDialogViewModel : BindableBase
    {
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

        public List<object> SelectedSeats { get; set; } = new();

        public ESplitOrderConditions SplitCondition { get; set; }

        public decimal SplitValue { get; set; }

        public decimal SplitTotal => Seats == null || Seats.Count() == 0
            ? 0
            : Seats.Sum(x => x.SelectedItem.TotalPrice);

        public bool IsSplitAvailable { get; set; }

        public bool IsKeyboardEnabled { get; set; }

        public bool IsNextStepAvailable { get; set; }

        public string HeaderText { get; set; }

        public EStep PopupNavigationStep { get; set; } = EStep.First;

        public Action<IDialogParameters> RequestClose;

        private ICommand _selectValueCommand;
        public ICommand SelectValueCommand => _selectValueCommand ??= new AsyncCommand<object>(OnSelectValueCommandAsync, allowsMultipleExecutions: false);

        private ICommand _incrementSplitValueCommand;
        public ICommand IncrementSplitValueCommand => _incrementSplitValueCommand ??= new AsyncCommand(OnIncrementSplitValueCommandAsync, allowsMultipleExecutions: false);

        private ICommand _decrementSplitValueCommand;
        public ICommand DecrementSplitValueCommand => _decrementSplitValueCommand ??= new AsyncCommand(OnDecrementSplitValueCommandAsync, allowsMultipleExecutions: false);

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(() => RequestClose(new DialogParameters { }));

        private ICommand _splitCommand;
        public ICommand SplitCommand => _splitCommand ??= new AsyncCommand(OnSplitCommandAsync, allowsMultipleExecutions: false);

        private ICommand _selectCommand;
        public ICommand SelectCommand => _selectCommand ??= new Command(OnSelectCommand);

        private ICommand _nextCommand;
        public ICommand NextCommand => _nextCommand ??= new AsyncCommand(OnNextCommand, allowsMultipleExecutions: false);

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SplitValue))
            {
                if (SplitCondition == ESplitOrderConditions.ByPercents)
                {
                    CalculateByPercentage();
                }
                else if (SplitCondition == ESplitOrderConditions.ByDollar)
                {
                    CalculateByDollar();
                }

                IsSplitAvailable = SplitTotal > 0;
            }
        }

        #endregion

        #region -- Private Helpers --

        private void LoadData(IDialogParameters param)
        {
            var isAllParamExist = param.TryGetValue(Constants.DialogParameterKeys.SEATS, out ObservableCollection<SeatBindableModel> seats)
                & param.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish)
                & param.TryGetValue(Constants.DialogParameterKeys.CONDITION, out ESplitOrderConditions condition);

            if (isAllParamExist)
            {
                SplitCondition = condition;
                SelectedDish = selectedDish;
                IEnumerable<SeatBindableModel> newSeats;

                if (condition == ESplitOrderConditions.BySeats)
                {
                    HeaderText = LocalizationResourceManager.Current["SplitBySeats"];

                    newSeats = seats.Select(x => new SeatBindableModel()
                    {
                        SeatNumber = x.SeatNumber,
                        SelectedItem = new DishBindableModel()
                        {
                            TotalPrice = x.SelectedDishes.Sum(x => x.TotalPrice),
                        },
                    });
                }
                else
                {
                    HeaderText = condition == ESplitOrderConditions.ByPercents
                        ? LocalizationResourceManager.Current["SplitByPercentage"]
                        : LocalizationResourceManager.Current["SplitByDollar"];

                    newSeats = seats.Where(x => x.Checked is false).Select(x => new SeatBindableModel()
                    {
                        SeatNumber = x.SeatNumber,
                        SelectedItem = new DishBindableModel()
                        {
                            TotalPrice = 0,
                        },
                    });
                }

                Seats = new ObservableCollection<SeatBindableModel>(newSeats);
            }
        }

        private Task OnSplitCommandAsync()
        {
            if (IsSplitAvailable)
            {
                if (SplitCondition == ESplitOrderConditions.BySeats)
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
            if (SelectedSeats.Count > 0)
            {
                var price = SplitValue * SelectedDish.TotalPrice / 100m;
                var selectedSeats = SelectedSeats.Select(x => x as SeatBindableModel);

                foreach (var seat in selectedSeats)
                {
                    seat.SelectedItem.TotalPrice = Math.Round(price, 2);
                }

                RaisePropertyChanged(nameof(SplitTotal));
            }
        }

        private void CalculateByDollar()
        {
            if (SelectedSeats.Count > 0)
            {
                var numberOfSelectedSeats = SelectedSeats.Count;
                var selectedSeats = SelectedSeats.Select(x => x as SeatBindableModel);
                var otherSeats = Seats.Except(selectedSeats);
                var splitTotalPrise = (SplitValue * numberOfSelectedSeats) + otherSeats.Sum(x => x.SelectedItem.TotalPrice);

                if (splitTotalPrise <= SelectedDish.TotalPrice)
                {
                    foreach (var seat in selectedSeats)
                    {
                        seat.SelectedItem.TotalPrice = Math.Round(SplitValue, 2);
                    }

                    RaisePropertyChanged(nameof(SplitTotal));
                }
            }
        }

        private Task OnSelectValueCommandAsync(object? arg)
        {
            decimal.TryParse(arg as string, out decimal value);
            var numberOfSelectedSeats = SelectedSeats.Count;
            var price = value * SelectedDish.TotalPrice / 100m;
            var selectedSeats = SelectedSeats.Select(x => x as SeatBindableModel);
            var otherSeats = Seats.Except(selectedSeats);
            var splitTotalPrise = (price * numberOfSelectedSeats) + otherSeats.Sum(x => x.SelectedItem.TotalPrice);

            if (splitTotalPrise <= SelectedDish.TotalPrice)
            {
                SplitValue = value;
            }

            return Task.CompletedTask;
        }

        private Task OnIncrementSplitValueCommandAsync()
        {
            if (SplitValue < Constants.Limits.MAX_PERCENTAGE)
            {
                var numberOfSelectedSeats = SelectedSeats.Count;
                var price = (SplitValue + 1) * SelectedDish.TotalPrice / 100m;
                var selectedSeats = SelectedSeats.Select(x => x as SeatBindableModel);
                var otherSeats = Seats.Except(selectedSeats);
                var splitTotalPrise = (price * numberOfSelectedSeats) + otherSeats.Sum(x => x.SelectedItem.TotalPrice);

                if (splitTotalPrise <= SelectedDish.TotalPrice)
                {
                    SplitValue++;
                }
            }

            return Task.CompletedTask;
        }

        private Task OnDecrementSplitValueCommandAsync()
        {
            if (SplitValue > 0)
            {
                SplitValue--;
            }

            return Task.CompletedTask;
        }

        private void OnSelectCommand()
        {
            IsNextStepAvailable = SelectedSeats.Count > 0;
            IsKeyboardEnabled = SelectedSeats.Count > 0;

            if (SplitCondition == ESplitOrderConditions.BySeats)
            {
                IsSplitAvailable = SelectedSeats.Count > 0;
            }
        }

        private Task OnNextCommand()
        {
            if (IsNextStepAvailable)
            {
                PopupNavigationStep = EStep.Second;
                IsNextStepAvailable = false;
            }

            return Task.CompletedTask;
        }

        private Task OnGoBackCommand()
        {
            PopupNavigationStep = EStep.First;
            IsNextStepAvailable = SelectedSeats.Count > 0;
            return Task.CompletedTask;
        }

        #endregion
    }
}
