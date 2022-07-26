using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Prism.Services.Dialogs;
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
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        public IPopupAnimation Animation { get; set; } = new Rg.Plugins.Popup.Animations.MoveAnimation(MoveAnimationOptions.Bottom, MoveAnimationOptions.Top);

        public int CursorPosition { get; set; }

        public string SearchLine { get; set; } = "Hello world";

        public string Placeholder { get; set; } = string.Empty;

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync()
        {
            Animation = new Rg.Plugins.Popup.Animations.MoveAnimation(MoveAnimationOptions.Top, MoveAnimationOptions.Bottom);

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            RequestClose(new DialogParameters());
        }

        #endregion
    }
}
