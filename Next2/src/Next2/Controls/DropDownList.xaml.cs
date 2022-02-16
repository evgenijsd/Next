using Next2.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
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

        public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(HeaderBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public Color HeaderBackgroundColor
        {
            get => (Color)GetValue(HeaderBackgroundColorProperty);
            set => SetValue(HeaderBackgroundColorProperty, value);
        }

        public static readonly BindableProperty HeaderFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(HeaderFontFamily),
            returnType: typeof(string),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public string HeaderFontFamily
        {
            get => (string)GetValue(HeaderFontFamilyProperty);
            set => SetValue(HeaderFontFamilyProperty, value);
        }

        public static readonly BindableProperty HeaderTextSizeProperty = BindableProperty.Create(
            propertyName: nameof(HeaderTextSize),
            returnType: typeof(int),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public int HeaderTextSize
        {
            get => (int)GetValue(HeaderTextSizeProperty);
            set => SetValue(HeaderTextSizeProperty, value);
        }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create(
            propertyName: nameof(HeaderText),
            returnType: typeof(string),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly BindableProperty WrappedListIconSourceProperty = BindableProperty.Create(
            propertyName: nameof(WrappedListIconSource),
            returnType: typeof(string),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public string WrappedListIconSource
        {
            get => (string)GetValue(WrappedListIconSourceProperty);
            set => SetValue(WrappedListIconSourceProperty, value);
        }

        public static readonly BindableProperty ExpandedListIconSourceProperty = BindableProperty.Create(
            propertyName: nameof(ExpandedListIconSource),
            returnType: typeof(string),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public string ExpandedListIconSource
        {
            get => (string)GetValue(ExpandedListIconSourceProperty);
            set => SetValue(ExpandedListIconSourceProperty, value);
        }

        public static readonly BindableProperty DataTemplateProperty = BindableProperty.Create(
            propertyName: nameof(DataTemplate),
            returnType: typeof(DataTemplate),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public DataTemplate DataTemplate
        {
            get => (DataTemplate)GetValue(DataTemplateProperty);
            set => SetValue(DataTemplateProperty, value);
        }

        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
            propertyName: nameof(ItemHeight),
            returnType: typeof(int),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public int ItemHeight
        {
            get => (int)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
         propertyName: nameof(ItemsSource),
         returnType: typeof(IList),
         declaringType: typeof(DropDownList),
         defaultBindingMode: BindingMode.TwoWay);

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(object),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
            propertyName: nameof(IsExpanded),
            returnType: typeof(bool),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private ICommand _expandListCommand;
        public ICommand ExpandListCommand => _expandListCommand ??= new AsyncCommand(OnExpandListCommandAsync);

        private ICommand _selectItemCommand;
        public ICommand SelectItemCommand => _selectItemCommand ??= new AsyncCommand(OnSelectItemCommandAsync);

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ItemsSource) && ItemsSource?.Count > 0)
            {
                SelectedItem = ItemsSource[0];
                int h = ItemHeight;
            }
        }

        #region -- Private helpers --

        private Task OnExpandListCommandAsync()
        {
            IsExpanded = !IsExpanded;

            (Parent as Layout).RaiseChild(this);

            return Task.CompletedTask;
        }

        private Task OnSelectItemCommandAsync()
        {
            IsExpanded = false;

            return Task.CompletedTask;
        }

        #endregion
    }
}