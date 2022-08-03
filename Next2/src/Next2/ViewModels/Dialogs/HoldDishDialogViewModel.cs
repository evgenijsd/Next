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
        public HoldDishDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            CurrentTime = DateTime.Now;
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTick);
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.CANCEL, true } }));
            DismissCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.DISMISS, true } }));
            HoldCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.HOLD, HoldTime } }));

            if (parameters.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish))
            {
                SelectedDish = selectedDish;

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

            InitTimeItemsAsync();
        }

        #region -- Public properties --

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<TimeItem> TimeDisplayItems { get; set; } = new();

        public TimeItem? SelectedTimeItem { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime HoldTime { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public string SetNames { get; set; } = string.Empty;

        public ICommand CloseCommand { get; }

        public ICommand DismissCommand { get; }

        public ICommand HoldCommand { get; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _tapTimeItemCommand;
        public ICommand TapTimeItemCommand => _tapTimeItemCommand = new AsyncCommand<TimeItem?>(OnTapTimeItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changingTimeHoldCommand;
        public ICommand ChangingTimeHoldCommand => _changingTimeHoldCommand ??= new AsyncCommand<EHoldChange>(OnChangingTimeHoldCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnTapTimeItemCommandAsync(TimeItem? sender)
        {
            var i = sender;

            return Task.CompletedTask;
        }

        private Task OnChangingTimeHoldCommandAsync(EHoldChange holdChange)
        {
            SelectedTimeItem = null;

            switch (holdChange)
            {
                case EHoldChange.HoursIncrement:
                    Hours++;
                    Hours = Hours > 24
                        ? 24
                        : Hours;

                    break;
                case EHoldChange.HoursDecrement:
                    Hours--;
                    Hours = Hours < 0
                        ? 0
                        : Hours;

                    break;
                case EHoldChange.MinuteIncrement:
                    Minutes++;
                    Minutes = Minutes > 60
                        ? 60
                        : Minutes;

                    break;
                case EHoldChange.MinuteDecrement:
                    Minutes--;
                    Minutes = Minutes < 0
                        ? 0
                        : Minutes;

                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        private void InitTimeItemsAsync()
        {
            TimeDisplayItems = new()
            {
                new()
                {
                    Minutes = 15,
                    TapCommand = TapTimeItemCommand,
                },
                new()
                {
                    Minutes = 20,
                    TapCommand = TapTimeItemCommand,
                },
                new()
                {
                    Minutes = 40,
                    TapCommand = TapTimeItemCommand,
                },
            };
        }

        private bool OnTimerTick()
        {
            CurrentTime = DateTime.Now;

            return true;
        }

        #endregion
    }
}
