using Next2.Enums;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Next2.ViewModels.Dialogs
{
    public class ConfirmViewModel : BindableBase
    {
        public ConfirmViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadPageData(param);
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
        }

        #region -- Public properties --

        public EConfirmMode ConfirmMode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CancellationText { get; set; }

        public string ConfirmationText { get; set; }

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; }

        public DelegateCommand DeclineCommand { get; }

        #endregion

        public Action<IDialogParameters> RequestClose;

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

        #endregion
    }
}
