using Next2.Enums;
using Next2.Helpers;
using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class HoldDishDialogViewModel : BindableBase
    {
        private DateTime _holdTime = DateTime.Now;
        private TimeItem? _preSelectedTimeItem;

        public HoldDishDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            CurrentTime = DateTime.Now;
            Hour = _holdTime.Hour;
            Minute = _holdTime.Minute;
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTick);

            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.CANCEL, true } }));
            DismissCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.DISMISS, true } }));

            if (parameters.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish))
            {
                SelectedDish = selectedDish;

                if (selectedDish.HoldTime is DateTime holdTime)
                {
                    _holdTime = holdTime;
                    Hour = _holdTime.Hour;
                    Minute = _holdTime.Minute;
                }

                foreach (var product in selectedDish.Products ?? new())
                {
                    if (!string.IsNullOrEmpty(product.Name))
                    {
                        SetNames = SetNames == string.Empty
                            ? SetNames = product.Name
                            : SetNames += $", {product.Name}";
                    }
                }
            }

            InitTimeItems();
        }

        #region -- Public properties --

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<TimeItem> TimeDisplayItems { get; set; } = new();

        public TimeItem? SelectedTimeItem { get; set; }

        public DateTime CurrentTime { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public string SetNames { get; set; } = string.Empty;

        public ICommand CloseCommand { get; }

        public ICommand DismissCommand { get; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _selectTimeItemCommand;
        public ICommand SelectTimeItemCommand => _selectTimeItemCommand ??= new AsyncCommand(OnSelectTimeItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changingTimeHoldCommand;
        public ICommand ChangingTimeHoldCommand => _changingTimeHoldCommand ??= new AsyncCommand<EHoldChange>(OnChangingTimeHoldCommandAsync, allowsMultipleExecutions: false);

        private ICommand _holdCommand;
        public ICommand HoldCommand => _holdCommand ??= new AsyncCommand(OnHoldCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnHoldCommandAsync()
        {
            var parameters = new DialogParameters();

            if (_holdTime > CurrentTime)
            {
                parameters.Add(Constants.DialogParameterKeys.HOLD, _holdTime);
            }

            RequestClose(parameters);

            return Task.CompletedTask;
        }

        private Task OnSelectTimeItemCommandAsync()
        {
            SelectedTimeItem = _preSelectedTimeItem == SelectedTimeItem
                ? null
                : SelectedTimeItem;

            _holdTime = DateTime.Now;
            _holdTime = _holdTime.AddMinutes(SelectedTimeItem?.Minute ?? 0);
            Hour = _holdTime.Hour;
            Minute = _holdTime.Minute;

            _preSelectedTimeItem = SelectedTimeItem;

            return Task.CompletedTask;
        }

        private Task OnChangingTimeHoldCommandAsync(EHoldChange holdChange)
        {
            var hour = Hour;
            var minute = Minute;

            switch (holdChange)
            {
                case EHoldChange.HourIncrement:
                    hour++;
                    hour = hour > 23
                        ? 23
                        : hour;

                    break;
                case EHoldChange.HourDecrement:
                    hour--;
                    hour = hour < 0
                        ? 0
                        : hour;

                    break;
                case EHoldChange.MinuteIncrement:
                    minute++;
                    minute = minute > 59
                        ? 59
                        : minute;

                    break;
                case EHoldChange.MinuteDecrement:
                    minute--;
                    minute = minute < 0
                        ? 0
                        : minute;

                    break;
                default:
                    break;
            }

            var holdTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, hour, minute, second: 0);

            if (holdTime >= CurrentTime)
            {
                Hour = hour;
                Minute = minute;
                _holdTime = holdTime;
            }
            else
            {
                _holdTime = DateTime.Now;
                Hour = _holdTime.Hour;
                Minute = _holdTime.Minute;
            }

            return Task.CompletedTask;
        }

        private void InitTimeItems()
        {
            TimeDisplayItems = new()
            {
                new()
                {
                    Minute = 15,
                },
                new()
                {
                    Minute = 20,
                },
                new()
                {
                    Minute = 40,
                },
            };
        }

        private bool OnTimerTick()
        {
            CurrentTime = DateTime.Now;
            var currentTime = CurrentTime.AddMinutes(SelectedTimeItem?.Minute ?? 0);

            if (_holdTime < currentTime)
            {
                _holdTime = currentTime;
                Hour = _holdTime.Hour;
                Minute = _holdTime.Minute;
            }

            return true;
        }

        #endregion
    }
}
