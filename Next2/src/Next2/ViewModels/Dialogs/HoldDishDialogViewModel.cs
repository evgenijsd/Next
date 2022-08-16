using Next2.Enums;
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
        public HoldDishDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            InitTimeItems();

            Device.StartTimer(TimeSpan.FromSeconds(Constants.Limits.HELD_DISH_RELEASE_FREQUENCY), OnTimerTick);

            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.CANCEL, true } }));
            DismissCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.DISMISS, true } }));

            if (parameters.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish))
            {
                SelectedDish = selectedDish;

                if (selectedDish.HoldTime is DateTime holdTime)
                {
                    HoldTime = holdTime;
                }

                ProductNames = string.Join(", ", selectedDish.Products.Where(x => !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToArray());
            }
        }

        #region -- Public properties --

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<HoldTimeItem> AvailableHoldingTimeInMinutes { get; set; }

        public HoldTimeItem? SelectedHoldingTimeInMinutes { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime HoldTime { get; set; }

        public string ProductNames { get; set; } = string.Empty;

        public ICommand CloseCommand { get; }

        public ICommand DismissCommand { get; }

        public Action<IDialogParameters> RequestClose;

        private ICommand? _selectTimeItemCommand;
        public ICommand SelectTimeItemCommand => _selectTimeItemCommand ??= new AsyncCommand<HoldTimeItem?>(OnSelectTimeItemCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _holdCommand;
        public ICommand HoldCommand => _holdCommand ??= new AsyncCommand(OnHoldCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnHoldCommandAsync()
        {
            var parameters = new DialogParameters();

            if (HoldTime > CurrentTime)
            {
                parameters.Add(Constants.DialogParameterKeys.HOLD, HoldTime);
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

                HoldTime = DateTime.Now;
                HoldTime = HoldTime.AddMinutes(SelectedHoldingTimeInMinutes?.Minute ?? 0);
            }

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

        private bool OnTimerTick()
        {
            CurrentTime = DateTime.Now;

            return true;
        }

        #endregion
    }
}
