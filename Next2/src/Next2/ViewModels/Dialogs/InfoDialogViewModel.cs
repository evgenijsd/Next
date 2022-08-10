using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class InfoDialogViewModel : BindableBase
    {
        public InfoDialogViewModel(
            DialogParameters parameters,
            Action requestClose)
        {
            LoadPageData(parameters);
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose());
        }

        #region -- Public properties --

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CloseText { get; set; } = string.Empty;

        public ICommand CloseCommand { get; }

        public Action RequestClose;

        #endregion

        #region -- Private helpers --

        private void LoadPageData(DialogParameters parameters)
        {
            if (parameters is not null)
            {
                if (parameters.TryGetValue(Constants.DialogParameterKeys.TITLE, out string title))
                {
                    Title = title;
                }

                if (parameters.TryGetValue(Constants.DialogParameterKeys.DESCRIPTION, out string description))
                {
                    Description = description;
                }

                if (parameters.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT, out string closeText))
                {
                    CloseText = closeText;
                }
            }
        }

        #endregion
    }
}
