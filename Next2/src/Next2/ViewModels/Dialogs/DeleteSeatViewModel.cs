using AutoMapper;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class DeleteSeatViewModel : BindableBase
    {
        public DeleteSeatViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            LoadDataFromParameters(param);
            RequestClose = requestClose;
            DeclineCommand = new Command(() => RequestClose(null));
            AcceptCommand = new Command(() => { });
        }

        #region -- Public properties --

        public string Text { get; set; }

        public bool IsDeletingItemsSelected { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _selectDeletingItemsCommand;
        public ICommand SelectDeletingItemsCommand => _selectDeletingItemsCommand ??= new Command(OnSelectDeletingItemsCommand);

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        #endregion

        #region -- Private helpers --

        private void LoadDataFromParameters(IDialogParameters param)
        {
            if (param.TryGetValue("text", out string text))
            {
                Text = text;
            }
        }

        private void OnSelectDeletingItemsCommand()
        {
            IsDeletingItemsSelected = !IsDeletingItemsSelected;
        }

        #endregion
    }
}