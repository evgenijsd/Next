using Next2.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class StepperCarousel : StackLayout
    {
        private int _firstVisibleItemIndex;
        private int _countVisibleItems;

        private double _viewItemWidth;

        public StepperCarousel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(StepperCarousel),
            default(IEnumerable<IBaseApiModel>));

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
            if (ItemsSource.Count - _firstVisibleItemIndex - _countVisibleItems > 1)
            {
                collectionView.ScrollTo(_firstVisibleItemIndex + 2, -1, ScrollToPosition.Start, true);
            }
        }

        private void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_viewItemWidth == 0f && collectionView.ItemTemplate.CreateContent() is View view)
            {
                _viewItemWidth = view.WidthRequest;

                _countVisibleItems = (int)((collectionView.Width / _viewItemWidth) * 2);
            }

            _firstVisibleItemIndex = (int)(e.HorizontalOffset / _viewItemWidth) * 2;
        }

        #endregion
    }
}