using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Next2.Controls
{
    public partial class DropDownList : PancakeView
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
            returnType: typeof(double),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly BindableProperty MaxNumberOfVisibleItemsProperty = BindableProperty.Create(
            propertyName: nameof(MaxNumberOfVisibleItems),
            returnType: typeof(int),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public int MaxNumberOfVisibleItems
        {
            get => (int)GetValue(MaxNumberOfVisibleItemsProperty);
            set => SetValue(MaxNumberOfVisibleItemsProperty, value);
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

        public double ListHeight { get; private set; }

        private ICommand _expandListCommand;
        public ICommand ExpandListCommand => _expandListCommand ??= new Command(OnExpandListCommand);

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (ItemsSource?.Count > 0 && propertyName
                is nameof(ItemsSource)
                or nameof(MaxNumberOfVisibleItems))
            {
                SelectedItem = ItemsSource[0];

                collectionList.VerticalScrollBarVisibility = ItemsSource.Count == MaxNumberOfVisibleItems
                    ? ScrollBarVisibility.Never
                    : ScrollBarVisibility.Always;
            }
            else if (propertyName == nameof(SelectedItem))
            {
                IsExpanded = false;
            }

            if (propertyName
                is nameof(MaxNumberOfVisibleItems)
                or nameof(ItemHeight)
                or nameof(ItemsSource))
            {
                if (ItemsSource is not null)
                {
                    ListHeight = ItemHeight * (ItemsSource.Count < MaxNumberOfVisibleItems
                        ? ItemsSource.Count
                        : MaxNumberOfVisibleItems);
                }
                else
                {
                    ListHeight = 0;
                }
            }
        }

        #region -- Private helpers --

        private void OnExpandListCommand()
        {
            IsExpanded = !IsExpanded;

            (Parent as Layout)?.RaiseChild(this);
        }

        #endregion
    }
}