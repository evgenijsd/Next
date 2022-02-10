using Next2.Enums;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class DropDownList : Frame
    {
        public DropDownList()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            propertyName: nameof(IconSource),
            returnType: typeof(string),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IEnumerable),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public bool IsExpanded { get; set; }

        private ICommand _expandListCommand;
        public ICommand ExpandListCommand => _expandListCommand ??= new AsyncCommand(OnExpandListCommandAsync);

        private ICommand _wrapListCommand;
        public ICommand WrapListCommand => _wrapListCommand ??= new AsyncCommand(OnWrapListCommandAsync);

        #endregion

        #region -- Private helpers --

        private Task OnExpandListCommandAsync()
        {
            IsExpanded = !IsExpanded;

            return Task.CompletedTask;
        }

        private Task OnWrapListCommandAsync()
        {
            IsExpanded = false;

            return Task.CompletedTask;
        }

        #endregion
    }
}