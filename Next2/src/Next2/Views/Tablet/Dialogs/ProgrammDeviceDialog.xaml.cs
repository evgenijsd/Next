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
                    var animation1 = new Animation(v => gearU.Rotation = v, 0, 4600);
                    var animation2 = new Animation(v => gearL.Rotation = v, 0, -4000);
                    var animation3 = new Animation(v => gearB.Rotation = v, 0, -3600);
                    var animation4 = new Animation(v => mechanism.Rotation = v, 0, 3600);

                    animation1.Commit(this, "RotateAnimation1", 8, 20000, Easing.Linear, (v, c) => gearU.Rotation = 0, () => true);
                    animation2.Commit(this, "RotateAnimation2", 8, 20000, Easing.Linear, (v, c) => gearL.Rotation = 0, () => true);
                    animation3.Commit(this, "RotateAnimation3", 8, 20000, Easing.Linear, (v, c) => gearB.Rotation = 0, () => true);
                    animation4.Commit(this, "RotateAnimation4", 8, 200000, Easing.Linear, (v, c) => mechanism.Rotation = 0, () => true);
                }
                else if ((EStep)stateContainer.State == EStep.Third)
                {
                    this.AbortAnimation("RotateAnimation1");
                    this.AbortAnimation("RotateAnimation2");
                    this.AbortAnimation("RotateAnimation3");
                    this.AbortAnimation("RotateAnimation4");
                }
            }
        }

        #endregion
    }
}