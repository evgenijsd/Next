using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class SelectDateDialogViewModel : BindableBase
    {
        public SelectDateDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            LoadData(param);
        }

        #region -- Public Properties --

        public DateTime? SelectedDate { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(() => RequestClose(new DialogParameters { }));

        private ICommand _selectCommand;
        public ICommand SelectCommand => _selectCommand ??= new Command(OnSelectCommand);

        #endregion

        #region -- Private Helpers --

        private void LoadData(DialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.SELECTED_DATE, out DateTime selectedDate))
            {
                SelectedDate = selectedDate;
            }
        }

        private void OnSelectCommand()
        {
            if (SelectedDate is not null)
            {
                var parameters = new DialogParameters()
                {
                    { Constants.DialogParameterKeys.SELECTED_DATE, SelectedDate },
                };

                RequestClose(parameters);
            }
        }

        #endregion

    }
}
