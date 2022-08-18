using Next2.Enums;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class ProgrammDeviceDialogViewModel : BindableBase
    {
        public ProgrammDeviceDialogViewModel(
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
        }

        #region -- Public Properties --

        public Action<IDialogParameters>? RequestClose;

        public EStep ProgrammingStep { get; set; } = EStep.First;

        private ICommand? _startProgrammingCommand;
        public ICommand StartProgrammingCommand => _startProgrammingCommand ??= new AsyncCommand(OnStartProgrammingCommand, allowsMultipleExecutions: false);

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(() => RequestClose(new DialogParameters { }));

        #endregion

        #region -- Private Helpers --

        private async Task OnStartProgrammingCommand()
        {
            Task delay = Task.Delay(10000);

            ProgrammingStep = EStep.Second;

            await delay;

            ProgrammingStep = EStep.Third;
        }

        #endregion
    }
}
