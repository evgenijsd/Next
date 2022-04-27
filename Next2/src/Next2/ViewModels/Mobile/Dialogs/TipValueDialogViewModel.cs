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
        public TipValueDialogViewModel(
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
        }

        #region -- Public properties --

        public string InputTip { get; set; } = string.Empty;

        public Action<IDialogParameters> RequestClose;

        private ICommand _closeCommand;

        public ICommand CloseCommand => _closeCommand ??= new AsyncCommand(OnCloseCommandAsync, allowsMultipleExecutions: false);

        private ICommand _getTipValueCommand;

        public ICommand GetTipValueCommand => _getTipValueCommand ??= new AsyncCommand(OnGetTipValueCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region --Private helpers--

        private Task OnCloseCommandAsync()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        private Task OnGetTipValueCommandAsync()
        {
            float tipValue;

            if (float.TryParse(InputTip, out float tip))
            {
                tipValue = tip / 100;
            }
            else
            {
                tipValue = 0;
            }

            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.TIP_VALUE, tipValue } };

            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        #endregion
    }
}
