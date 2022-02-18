using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomNavigationbar : Grid
    {
        public CustomNavigationbar()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty LeftButtonCommandProperty = BindableProperty.Create(
            propertyName: nameof(LeftButtonCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand LeftButtonCommand
        {
            get => (ICommand)GetValue(LeftButtonCommandProperty);
            set => SetValue(LeftButtonCommandProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
            propertyName: nameof(TitleColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public Color TitleColor
        {
            get => (Color)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        #endregion
    }
}