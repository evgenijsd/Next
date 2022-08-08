using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Dialogs
{
    public class TipValueDialogViewModel : BindableBase
    {
        public TipValueDialogViewModel(Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
        }

        #region -- Public properties --

        public string InputTip { get; set; } = string.Empty;

        public Action<IDialogParameters> RequestClose;

        private ICommand? _getTipValueCommand;
        public ICommand GetTipValueCommand => _getTipValueCommand ??= new AsyncCommand(OnGetTipValueCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnGetTipValueCommandAsync()
        {
            decimal tipValue = 0;

            if (decimal.TryParse(InputTip, out decimal tip))
            {
                tipValue = tip / 100;
            }

            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.TIP_VALUE_DIALOG, tipValue } };
            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        #endregion
    }
}
