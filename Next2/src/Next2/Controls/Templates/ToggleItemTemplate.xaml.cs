using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class ToggleItemTemplate : StackLayout
    {
        public ToggleItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty BindableLayoutProperty = BindableProperty.Create(
            propertyName: nameof(BindableLayout),
            returnType: typeof(object),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public object? BindableLayout
        {
            get => GetValue(BindableLayoutProperty);
            set => SetValue(BindableLayoutProperty, value);
        }

        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            propertyName: nameof(State),
            returnType: typeof(object),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public object? State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public static readonly BindableProperty IsToggleProperty = BindableProperty.Create(
            propertyName: nameof(IsToggle),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsToggle
        {
            get => (bool)GetValue(IsToggleProperty);
            set => SetValue(IsToggleProperty, value);
        }

        public static readonly BindableProperty CanTurnOffProperty = BindableProperty.Create(
            propertyName: nameof(CanTurnOff),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public bool CanTurnOff
        {
            get => (bool)GetValue(CanTurnOffProperty);
            set => SetValue(CanTurnOffProperty, value);
        }

        public static readonly BindableProperty IsVisibleSubtitleProperty = BindableProperty.Create(
            propertyName: nameof(IsVisibleSubtitle),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsVisibleSubtitle
        {
            get => (bool)GetValue(IsVisibleSubtitleProperty);
            set => SetValue(IsVisibleSubtitleProperty, value);
        }

        public static readonly BindableProperty IsVisibleImageProperty = BindableProperty.Create(
            propertyName: nameof(IsVisibleImage),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsVisibleImage
        {
            get => (bool)GetValue(IsVisibleImageProperty);
            set => SetValue(IsVisibleImageProperty, value);
        }

        public static readonly BindableProperty ImagePathProperty = BindableProperty.Create(
            propertyName: nameof(ImagePath),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public static readonly BindableProperty ImageSizesProperty = BindableProperty.Create(
            propertyName: nameof(ImageSizes),
            returnType: typeof(double),
            declaringType: typeof(ToggleItemTemplate),
            defaultValue: 50d,
            defaultBindingMode: BindingMode.OneWay);

        public double ImageSizes
        {
            get => (double)GetValue(ImageSizesProperty);
            set => SetValue(ImageSizesProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
            propertyName: nameof(TitleFontSize),
            returnType: typeof(double),
            declaringType: typeof(ToggleItemTemplate),
            defaultValue: 20d,
            defaultBindingMode: BindingMode.OneWay);

        public double TitleFontSize
        {
            get => (double)GetValue(TitleFontSizeProperty);
            set => SetValue(TitleFontSizeProperty, value);
        }

        public static readonly BindableProperty TitleFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(TitleFontFamily),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate),
            defaultValue: "Barlow-SemiBold",
            defaultBindingMode: BindingMode.OneWay);

        public string TitleFontFamily
        {
            get => (string)GetValue(TitleFontFamilyProperty);
            set => SetValue(TitleFontFamilyProperty, value);
        }

        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(
            propertyName: nameof(Subtitle),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public static readonly BindableProperty SubtitleFontSizeProperty = BindableProperty.Create(
            propertyName: nameof(SubtitleFontSize),
            returnType: typeof(double),
            declaringType: typeof(ToggleItemTemplate),
            defaultValue: 14d,
            defaultBindingMode: BindingMode.OneWay);

        public double SubtitleFontSize
        {
            get => (double)GetValue(SubtitleFontSizeProperty);
            set => SetValue(SubtitleFontSizeProperty, value);
        }

        public static readonly BindableProperty SubtitleFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(SubtitleFontFamily),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate),
            defaultValue: "Barlow-SemiBold",
            defaultBindingMode: BindingMode.OneWay);

        public string SubtitleFontFamily
        {
            get => (string)GetValue(SubtitleFontFamilyProperty);
            set => SetValue(SubtitleFontFamilyProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(State):
                case nameof(BindableLayout):
                    IsToggle = State?.ToString() == BindableLayout?.ToString();

                    break;
                case nameof(IsToggle):
                    if (IsToggle)
                    {
                        BindableLayout = State;
                    }

                    break;
            }
        }

        #endregion
    }
}