using Next2.Enums;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading;
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

        public double ImageScale { get; set; } = 1;

        public double ImageRotation { get; set; } = 0;

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

            Timer t = new Timer(new TimerCallback(PulseImage), null, 0, 80);
            Timer t2 = new Timer(new TimerCallback(RotateImage), null, 0, 100);

            await delay;

            ProgrammingStep = EStep.Third;

            t.Dispose();
            t2.Dispose();
        }

        private void PulseImage(object state)
        {
            if (ImageScale == 1)
            {
                ImageScale = 0.98;
            }
            else
            {
                ImageScale = 1;
            }
        }

        private void RotateImage(object state)
        {
            ImageRotation += 10;
        }

        #endregion
    }
}
