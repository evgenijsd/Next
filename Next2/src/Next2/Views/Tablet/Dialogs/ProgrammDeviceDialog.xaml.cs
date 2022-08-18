using Next2.Controls.StateContainer;
using Next2.Enums;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.ComponentModel;
using System.Threading;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class ProgrammDeviceDialog : PopupPage
    {
        private Timer t;
        private Timer t2;
        public ProgrammDeviceDialog(Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new ProgrammDeviceDialogViewModel(requestClose);

            t = new(new TimerCallback(PulseImage));
            t2 = new(new TimerCallback(RotateImage));
        }

        #region -- Private helpers --

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "State"
                && sender is StateContainer stateContainer
                && (EStep)stateContainer.State != EStep.First)
            {
                if ((EStep)stateContainer.State == EStep.Second)
                {
                    t.Change(0, 80);
                    t2.Change(0, 100);
                }
                else if ((EStep)stateContainer.State == EStep.Third)
                {
                    t.Dispose();
                    t2.Dispose();
                }
            }
        }

        private void PulseImage(object state)
        {
            if (settingsImage.Scale == 1)
            {
                settingsImage.Scale = 0.98;
            }
            else
            {
                settingsImage.Scale = 1;
            }
        }

        private void RotateImage(object state)
        {
            settingsImage.Rotation += 10;
        }

        #endregion
    }
}