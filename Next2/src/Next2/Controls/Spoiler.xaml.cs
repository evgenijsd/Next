using Next2.Helpers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class Spoiler : StackLayout
    {
        public Spoiler()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ICollection),
            typeof(Spoiler),
            default(ICollection),
            defaultBindingMode: BindingMode.OneWay);

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(SpoilerItem),
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.TwoWay);

        public SpoilerItem SelectedItem
        {
            get => (SpoilerItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty SelectionChangedCommandProperty = BindableProperty.Create(
            propertyName: nameof(SelectionChangedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.OneWay);

        public ICommand SelectionChangedCommand
        {
            get => (ICommand)GetValue(SelectionChangedCommandProperty);
            set => SetValue(SelectionChangedCommandProperty, value);
        }

        public static readonly BindableProperty SelectionChangedCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(SelectionChangedCommandParameter),
            returnType: typeof(object),
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.OneWay);

        public object SelectionChangedCommandParameter
        {
            get => (object)GetValue(SelectionChangedCommandParameterProperty);
            set => SetValue(SelectionChangedCommandParameterProperty, value);
        }

        public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(
            propertyName: nameof(HeaderTemplate),
            returnType: typeof(View),
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.OneWay);

        public View HeaderTemplate
        {
            get => (View)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            propertyName: nameof(ItemTemplate),
            returnType: typeof(DataTemplate),
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.OneWay);

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty IsRolledProperty = BindableProperty.Create(
            propertyName: nameof(IsRolled),
            returnType: typeof(bool),
            defaultValue: true,
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsRolled
        {
            get => (bool)GetValue(IsRolledProperty);
            set => SetValue(IsRolledProperty, value);
        }

        public static readonly BindableProperty HeightListProperty = BindableProperty.Create(
            propertyName: nameof(HeightList),
            returnType: typeof(double),
            declaringType: typeof(Spoiler),
            defaultBindingMode: BindingMode.TwoWay);

        public double HeightList
        {
            get => (double)GetValue(HeightListProperty);
            set => SetValue(HeightListProperty, value);
        }

        private ICommand _tapSpoilerCommand;
        public ICommand TapSpoilerCommand => _tapSpoilerCommand ??= new Command(OnTapSpoilerCommand);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if ((propertyName is nameof(ItemsSource) or nameof(ItemTemplate))
                && ItemTemplate is not null
                && ItemsSource is not null)
            {
                if (ItemTemplate.CreateContent() is View view)
                {
                    HeightList = ItemsSource.Count * view.HeightRequest;
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnTapSpoilerCommand()
        {
            IsRolled = !IsRolled;
        }

        #endregion
    }
}