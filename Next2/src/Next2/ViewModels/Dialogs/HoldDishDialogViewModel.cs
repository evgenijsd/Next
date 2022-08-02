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
            //LoadPageData(parameters);
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.CANCEL, true } }));
            DismissCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.DISMISS, true } }));
            HoldCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.HOLD, HoldTime } }));

            if (parameters.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel selectedDish))
            {
                SelectedDish = selectedDish;
            }

            InitTimeItemsAsync();
        }

        #region -- Public properties --

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<TimeItem> TimeDisplayItems { get; set; } = new();

        public TimeItem SelectedTimeItem { get; set; }

        public DateTime HoldTime { get; set; }

        public ICommand CloseCommand { get; }

        public ICommand DismissCommand { get; }

        public ICommand HoldCommand { get; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _tapTimeItemCommand;
        public ICommand TapTimeItemCommand => _tapTimeItemCommand = new AsyncCommand<object?>(OnTapTimeItemCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnTapTimeItemCommandAsync(object? sender)
        {
            var i = sender;

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

        #endregion
    }
}
