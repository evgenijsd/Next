using Next2.Helpers;
using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class HoldDishDialogViewModel : BindableBase
    {
        private DateTime _holdTime;

        public HoldDishDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            InitTimeItems();

            SetHoldTime(DateTime.Now);

            Device.StartTimer(TimeSpan.FromSeconds(Constants.Limits.HELD_DISH_RELEASE_FREQUENCY), OnCurrentTimeCheckHoldTimerTick);

            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.CANCEL, true } }));

            if (parameters.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish))
            {
                SelectedDish = selectedDish;

                ProductNames = string.Join(", ", selectedDish.SelectedProducts?.Where(x => !string.IsNullOrEmpty(x.Product.Name)).Select(x => x.Product.Name).ToArray());
            }

            if (parameters.TryGetValue(Constants.DialogParameterKeys.ORDER, out FullOrderBindableModel order))
            {
                var seatNumbers = order.Seats.Select(x => x.SeatNumber).ToList();

                seatNumbers.Insert(0, Constants.Limits.ALL_SEATS);

                SeatNumbers = new(seatNumbers);
                SeatNumber = SeatNumbers.First();
            }
        }

        #region -- Public properties --

        public DishBindableModel? SelectedDish { get; set; }

        public ObservableCollection<int>? SeatNumbers { get; set; }

        public int SeatNumber { get; set; }

        public int CurrentSeatNumber { get; set; }

        public FullOrderBindableModel CurrentOrder { get; set; }

        public ObservableCollection<HoldTimeItem> AvailableHoldingTimeInMinutes { get; set; }

        public HoldTimeItem? SelectedHoldingTimeInMinutes { get; set; }

        public DateTime CurrentTime { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public string ProductNames { get; set; } = string.Empty;

        public ICommand CloseCommand { get; }

        public ICommand DismissCommand { get; }

        public Action<IDialogParameters> RequestClose;

        private Action? _updateHoldTime;
        public Action? UpdateHoldTime => _updateHoldTime ??= new Action(OnUpdateHoldTime);

        private ICommand? _selectTimeItemCommand;
        public ICommand SelectTimeItemCommand => _selectTimeItemCommand ??= new AsyncCommand<HoldTimeItem?>(OnSelectTimeItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectSeatCommand;
        public ICommand SelectSeatCommand => _selectSeatCommand ??= new AsyncCommand(OnSelectSeatCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _holdCommand;
        public ICommand HoldCommand => _holdCommand ??= new AsyncCommand(OnHoldCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private void OnUpdateHoldTime()
        {
            var holdTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, Hour, Minute, second: 0, DateTimeKind.Local);

            if (CurrentTime > holdTime)
            {
                SetHoldTime(DateTime.Now);
            }
            else
            {
                _holdTime = holdTime;
            }
        }

        private Task OnHoldCommandAsync()
        {
            var parameters = new DialogParameters();

            if (_holdTime > CurrentTime)
            {
                parameters.Add(Constants.DialogParameterKeys.HOLD, _holdTime);

                if (SelectedDish is null)
                {
                    parameters.Add(Constants.DialogParameterKeys.SEATS, SeatNumber);
                }
            }

            RequestClose(parameters);

            return Task.CompletedTask;
        }

        private Task OnSelectTimeItemCommandAsync(HoldTimeItem? selectedHoldTime)
        {
            if (selectedHoldTime is not null)
            {
                selectedHoldTime.IsSelected = true;

                if (SelectedHoldingTimeInMinutes is not null)
                {
                    SelectedHoldingTimeInMinutes.IsSelected = false;
                    SelectedHoldingTimeInMinutes = selectedHoldTime == SelectedHoldingTimeInMinutes
                        ? null
                        : selectedHoldTime;
                }
                else
                {
                    SelectedHoldingTimeInMinutes = selectedHoldTime;
                }

                if (SelectedHoldingTimeInMinutes is not null)
                {
                    SetHoldTime(DateTime.Now.AddMinutes(selectedHoldTime.Minute));
                }
            }

            return Task.CompletedTask;
        }

        private Task OnSelectSeatCommandAsync()
        {
            CurrentSeatNumber = SeatNumber;

            return Task.CompletedTask;
        }

        private void InitTimeItems()
        {
            AvailableHoldingTimeInMinutes = new()
            {
                new()
                {
                    Minute = 15,
                    TapCommand = SelectTimeItemCommand,
                },
                new()
                {
                    Minute = 20,
                    TapCommand = SelectTimeItemCommand,
                },
                new()
                {
                    Minute = 40,
                    TapCommand = SelectTimeItemCommand,
                },
            };
        }

        private bool OnCurrentTimeCheckHoldTimerTick()
        {
            CurrentTime = DateTime.Now;

            if (CurrentTime > _holdTime)
            {
                SetHoldTime(DateTime.Now);
            }

            return true;
        }

        private void SetHoldTime(DateTime holdTime)
        {
            _holdTime = holdTime;
            Hour = _holdTime.Hour;
            Minute = _holdTime.Minute;
        }

        #endregion
    }
}
