using Next2.Controls.Templates;
using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class StepperCarousel : StackLayout
    {
        private int firstVisibleItemIndex;

        public ICommand TapLeftButtonCommand => new Command(OnToachStartedLeftButtonCommand);

        public ICommand TapRightButtonCommand => new Command(OnToachStartedRightButtonCommand);

        public ICommand ScrolledCommand => new Command<int>(OnScrolledCommand);

        public StepperCarousel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<ISelectable>),
            typeof(StepperCarousel),
            default(IEnumerable<ISelectable>));

        public IEnumerable<ISelectable> ItemsSource
        {
            get => (IEnumerable<ISelectable>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(ISelectable),
            declaringType: typeof(StepperCarousel),
            default(ISelectable),
            defaultBindingMode: BindingMode.TwoWay);

        public ISelectable SelectedItem
        {
            get => (ISelectable)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion

        #region --Private methods--

        private void OnToachStartedLeftButtonCommand(object obj)
        {
            if (firstVisibleItemIndex > 1)
            {
                collectionView.ScrollTo(firstVisibleItemIndex - 2, -1, ScrollToPosition.Start, true);
            }
        }

        private void OnToachStartedRightButtonCommand(object obj)
        {
            collectionView.ScrollTo(firstVisibleItemIndex + 2, -1, ScrollToPosition.Start, true);
        }

        private void OnScrolledCommand(int firstVisibleItemIdx)
        {
            firstVisibleItemIndex = firstVisibleItemIdx;
        }

        #endregion
    }
}