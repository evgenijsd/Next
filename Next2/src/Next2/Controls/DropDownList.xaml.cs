using Next2.Enums;
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

        public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(
            propertyName: nameof(HeaderTextColor),
            returnType: typeof(Color),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public Color HeaderTextColor
        {
            get => (Color)GetValue(HeaderTextColorProperty);
            set => SetValue(HeaderTextColorProperty, value);
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
            returnType: typeof(double),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public double HeaderTextSize
        {
            get => (double)GetValue(HeaderTextSizeProperty);
            set => SetValue(HeaderTextSizeProperty, value);
        }

        public static readonly BindableProperty HeaderTextOpacityProperty = BindableProperty.Create(
            propertyName: nameof(HeaderTextOpacity),
            returnType: typeof(double),
            declaringType: typeof(DropDownList),
            defaultValue: 1d,
            defaultBindingMode: BindingMode.TwoWay);

        public double HeaderTextOpacity
        {
            get => (double)GetValue(HeaderTextOpacityProperty);
            set => SetValue(HeaderTextOpacityProperty, value);
        }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create(
            propertyName: nameof(HeaderText),
            returnType: typeof(FormattedString),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public FormattedString HeaderText
        {
            get => (FormattedString)GetValue(HeaderTextProperty);
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

        public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
            propertyName: nameof(IconSizes),
            returnType: typeof(int),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public int IconSizes
        {
            get => (int)GetValue(IconSizesProperty);
            set => SetValue(IconSizesProperty, value);
        }

        public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
            propertyName: nameof(Direction),
            returnType: typeof(EDropDownListDirection),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public EDropDownListDirection Direction
        {
            get => (EDropDownListDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public static readonly BindableProperty ScrollBarVisibilityProperty = BindableProperty.Create(
            propertyName: nameof(ScrollBarVisibility),
            returnType: typeof(ScrollBarVisibility),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public ScrollBarVisibility ScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(ScrollBarVisibilityProperty);
            set => SetValue(ScrollBarVisibilityProperty, value);
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

        public static readonly BindableProperty ShouldTranslationYProperty = BindableProperty.Create(
            propertyName: nameof(ShouldTranslationY),
            returnType: typeof(bool),
            declaringType: typeof(DropDownList),
            defaultBindingMode: BindingMode.TwoWay);

        public bool ShouldTranslationY
        {
            get => (bool)GetValue(ShouldTranslationYProperty);
            set => SetValue(ShouldTranslationYProperty, value);
        }

        public static readonly BindableProperty SelectionChangedCommandProperty = BindableProperty.Create(
            propertyName: nameof(SelectionChangedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(DropDownList));

        public ICommand SelectionChangedCommand
        {
            get => (ICommand)GetValue(SelectionChangedCommandProperty);
            set => SetValue(SelectionChangedCommandProperty, value);
        }

        public double ListHeight { get; private set; }

        private ICommand _selectItemCommand;
        public ICommand SelectItemCommand => _selectItemCommand ??= new Command(OnSelectItemCommand);

        private ICommand _expandListCommand;
        public ICommand ExpandListCommand => _expandListCommand ??= new Command(OnExpandListCommand);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(SelectedItem))
            {
                IsExpanded = false;
            }
            else if (propertyName is nameof(Direction) && Direction is EDropDownListDirection.Up)
            {
                container.RaiseChild(listHeader);
            }
            else if (propertyName is nameof(IsExpanded) && ShouldTranslationY && Direction is EDropDownListDirection.Up)
            {
                dropDownList.TranslationY = IsExpanded
                    ? -ListHeight
                    : 0;
            }
            else if (propertyName
                is nameof(ItemsSource)
                or nameof(MaxNumberOfVisibleItems) && ItemsSource?.Count > 0)
            {
                if (ScrollBarVisibility is not ScrollBarVisibility.Never)
                {
                    itemsCollection.VerticalScrollBarVisibility = ItemsSource.Count == MaxNumberOfVisibleItems
                        ? ScrollBarVisibility.Never
                        : ScrollBarVisibility;
                }
            }

            if (propertyName
                is nameof(ItemHeight)
                or nameof(ItemsSource)
                or nameof(MaxNumberOfVisibleItems))
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

            if (propertyName is nameof(IsExpanded) && IsExpanded)
            {
                itemsCollection.ScrollTo(itemsCollection.SelectedItem, position: ScrollToPosition.Center, animate: false);
            }

            base.OnPropertyChanging(propertyName);
        }

        #endregion

        #region -- Private helpers --

        private void OnSelectItemCommand() => IsExpanded = false;

        private void OnExpandListCommand()
        {
            IsExpanded = !IsExpanded;

            (Parent as Layout)?.RaiseChild(this);
        }

        #endregion
    }
}