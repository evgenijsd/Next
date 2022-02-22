using Prism.Mvvm;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            default(ICollection));

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(object),
            declaringType: typeof(Spoiler));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            propertyName: nameof(ItemTemplate),
            returnType: typeof(DataTemplate),
            declaringType: typeof(Spoiler));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(
            propertyName: nameof(HeaderTemplate),
            returnType: typeof(View),
            declaringType: typeof(Spoiler));

        public View HeaderTemplate
        {
            get => (View)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly BindableProperty IsRolledProperty = BindableProperty.Create(
            propertyName: nameof(IsRolled),
            returnType: typeof(bool),
            defaultValue: true,
            declaringType: typeof(Spoiler));

        public bool IsRolled
        {
            get => (bool)GetValue(IsRolledProperty);
            set => SetValue(IsRolledProperty, value);
        }

        public static readonly BindableProperty HeightListProperty = BindableProperty.Create(
            propertyName: nameof(HeightList),
            returnType: typeof(double),
            declaringType: typeof(Spoiler));

        public double HeightList
        {
            get => (double)GetValue(HeightListProperty);
            set => SetValue(HeightListProperty, value);
        }

        private ICommand _tapSpoilerCommand;
        public ICommand TapSpoilerCommand => _tapSpoilerCommand ??= new Command(OnTapSpoilerCommand);

        #endregion

        #region --Overrides--

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if ((propertyName == nameof(ItemsSource) || propertyName == nameof(ItemTemplate)) && ItemTemplate is not null && ItemsSource is not null)
            {
                var view = (View)ItemTemplate.CreateContent();
                HeightList = ItemsSource.Count * view.HeightRequest;
            }
        }

        #endregion

        #region --Private methods--

        private void OnTapSpoilerCommand()
        {
            IsRolled = !IsRolled;
        }

        #endregion
    }
}