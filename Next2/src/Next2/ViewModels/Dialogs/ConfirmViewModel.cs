using Next2.Enums;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class ConfirmViewModel : BindableBase
    {
        private bool _canExecute = true;

        public ConfirmViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadPageData(param);
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));
            DeclineCommand = new Command(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
            AcceptCommand = new Command(
                execute: () => RequestClose(new DialogParameters()
                {
                    { Constants.DialogParameterKeys.ACCEPT, true },
                }),
                canExecute: CanExecute);
        }

        #region -- Public properties --

        public EConfirmMode ConfirmMode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CancellationText { get; set; }

        public string ConfirmationText { get; set; }

        public string Parameter { get; set; }

        public ICommand CloseCommand { get; }

        public ICommand AcceptCommand { get; }

        public ICommand DeclineCommand { get; }

        public Action<IDialogParameters> RequestClose;

        #endregion

        #region -- Private helpers --

        private void LoadPageData(DialogParameters param)
        {
            if (param is not null)
            {
                if (param.TryGetValue(Constants.DialogParameterKeys.CONFIRM_MODE, out EConfirmMode confirmMode))
                {
                    ConfirmMode = confirmMode;
                }

                if (param.TryGetValue(Constants.DialogParameterKeys.TITLE, out string title))
                {
                    Title = title;
                }

                if (param.TryGetValue(Constants.DialogParameterKeys.DESCRIPTION, out string description))
                {
                    Description = description;
                }

                if (param.TryGetValue(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, out string cancellationText))
                {
                    CancellationText = cancellationText;
                }

                if (param.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT, out string confirmationText))
                {
                    ConfirmationText = confirmationText;
                }
            }
        }

        private bool CanExecute()
        {
            bool result = false;

            if (_canExecute)
            {
                _canExecute = false;
                result = true;
            }

            return result;
        }

        #endregion
    }
}
