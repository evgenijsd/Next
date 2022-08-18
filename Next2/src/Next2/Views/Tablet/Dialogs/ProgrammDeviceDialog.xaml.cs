using Next2.Controls.StateContainer;
using Next2.Enums;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class ProgrammDeviceDialog : PopupPage
    {
        public ProgrammDeviceDialog(Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new ProgrammDeviceDialogViewModel(requestClose);
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
                    var animation = new Animation(v => settingsImage.Scale = v, 1, 0.94);
                    var animation2 = new Animation(v => settingsImage.Rotation = v, 0, 3600);

                    animation.Commit(this, "PulseAnimation", 1, 80, Easing.SpringIn, (v, c) => settingsImage.Scale = 1, () => true);
                    animation2.Commit(this, "RotateAnimation", 1, 20000, Easing.Linear, (v, c) => settingsImage.Rotation = 0, () => true);
                }
                else if ((EStep)stateContainer.State == EStep.Third)
                {
                    this.AbortAnimation("PulseAnimation");
                    this.AbortAnimation("RotateAnimation");
                }
            }
        }

        #endregion
    }
}