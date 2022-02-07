using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class DialogsButton : Frame
    {
        public DialogsButton()
        {
            InitializeComponent();
        }

        #region --Public Properties--

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
          propertyName: nameof(Command),
          returnType: typeof(ICommand),
          declaringType: typeof(DialogsButton),
          defaultBindingMode: BindingMode.TwoWay);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
          propertyName: nameof(IsActive),
          returnType: typeof(bool),
          declaringType: typeof(DialogsButton),
          defaultValue: false,
          defaultBindingMode: BindingMode.TwoWay);

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
          propertyName: nameof(Text),
          returnType: typeof(string),
          declaringType: typeof(DialogsButton),
          defaultValue: string.Empty,
          defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
          propertyName: nameof(Color),
          returnType: typeof(Color),
          declaringType: typeof(DialogsButton),
          defaultBindingMode: BindingMode.TwoWay);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
          propertyName: nameof(BorderColor),
          returnType: typeof(Color),
          declaringType: typeof(DialogsButton),
          defaultBindingMode: BindingMode.TwoWay);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        #endregion

        #region --Overrides--

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged();
            if (IsActiveProperty.PropertyName == nameof(IsActive))
            {
                if (IsActive)
                {
                    dialogsButton.Opacity = 1;
                }
                else
                {
                    dialogsButton.Opacity = 0.32;
                }
            }
        }

        #endregion
    }
}