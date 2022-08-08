using Prism.Mvvm;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Interfaces.Animations;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class InputDialogViewModel : BindableBase
    {
        public InputDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            if (param.TryGetValue(Constants.Navigations.INPUT_VALUE, out string text))
            {
                Text = text;
                CursorPosition = Text.Length;
            }

            if (param.TryGetValue(Constants.Navigations.PLACEHOLDER, out string placeholder))
            {
                Placeholder = placeholder;
            }
        }

        #region -- Public properties --

        public IPopupAnimation Animation { get; set; } = new MoveAnimation(MoveAnimationOptions.Bottom, MoveAnimationOptions.Top);

        public int CursorPosition { get; set; }

        public string Text { get; set; } = string.Empty;

        public string Placeholder { get; set; } = string.Empty;

        public Action<IDialogParameters> RequestClose;

        private ICommand? _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnGoBackCommandAsync(string? text)
        {
            Animation = new MoveAnimation(MoveAnimationOptions.Top, MoveAnimationOptions.Bottom);

            var param = new DialogParameters();

            if (!string.IsNullOrEmpty(text))
            {
                param.Add(Constants.Navigations.INPUT_VALUE, Text);
            }

            RequestClose(param);

            return Task.CompletedTask;
        }

        #endregion
    }
}
