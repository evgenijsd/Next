using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
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
        }

        #region -- Public properties --

        public DishBindableModel SelectedDish { get; set; }

        public DateTime HoldTime { get; set; }

        public ICommand CloseCommand { get; }

        public ICommand DismissCommand { get; }

        public ICommand HoldCommand { get; }

        public Action<IDialogParameters> RequestClose;

        #endregion

        #region -- Private helpers --

        #endregion
    }
}
