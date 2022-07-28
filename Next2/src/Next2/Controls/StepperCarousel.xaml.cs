using Next2.Interfaces;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class StepperCarousel : StackLayout
    {
        private int _firstVisibleItemIndex;
        private int _countVisibleItems;

        public StepperCarousel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            declaringType: typeof(StepperCarousel),
            returnType: typeof(IList),
            defaultValue: default(IList),
            defaultBindingMode: BindingMode.OneWay);

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(IBaseApiModel),
            declaringType: typeof(StepperCarousel),
            defaultBindingMode: BindingMode.TwoWay);

        public IBaseApiModel SelectedItem
        {
            get => (IBaseApiModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
            propertyName: nameof(ItemWidth),
            returnType: typeof(double),
            defaultValue: 124d,
            declaringType: typeof(StepperCarousel),
            defaultBindingMode: BindingMode.OneWay);

        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        private ICommand _scrollToLeftCommand;
        public ICommand ScrollToLeftCommand => _scrollToLeftCommand ??= new Command(OnScrollToLeftCommand);

        private ICommand _scrollToRightCommand;
        public ICommand ScrollToRightCommand => _scrollToRightCommand ??= new Command(OnScrollToRightCommand);

        #endregion

        #region -- Private helpers --

        private void OnScrollToLeftCommand()
        {
            if (_firstVisibleItemIndex > 1)
            {
                collectionView.ScrollTo(_firstVisibleItemIndex - 2, -1, ScrollToPosition.Start, true);
            }
        }

        private void OnScrollToRightCommand()
        {
            if (ItemsSource.Count - _firstVisibleItemIndex - _countVisibleItems >= 1)
            {
                collectionView.ScrollTo(_firstVisibleItemIndex + 2, -1, ScrollToPosition.Start, true);
            }
        }

        private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_countVisibleItems <= 0f)
            {
                _countVisibleItems = (int)((collectionView.Width / ItemWidth) * 2);
            }

            _firstVisibleItemIndex = (int)(e.HorizontalOffset / ItemWidth) * 2;
        }

        #endregion
    }
}