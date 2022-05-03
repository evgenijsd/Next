﻿using Next2.Interfaces;
using System;
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
            typeof(IEnumerable<IBaseModel>),
            typeof(StepperCarousel),
            default(IEnumerable<IBaseModel>));

        public IEnumerable<IBaseModel> ItemsSource
        {
            get => (IEnumerable<IBaseModel>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(IBaseModel),
            declaringType: typeof(StepperCarousel),
            defaultBindingMode: BindingMode.TwoWay);

        public IBaseModel SelectedItem
        {
            get => (IBaseModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private ICommand _scrollRightCommand;
        public ICommand ScrollRightCommand => _scrollRightCommand ??= new Command(OnScrollRightCommand);

        private ICommand _scrollLeftCommand;
        public ICommand ScrollLeftCommand => _scrollLeftCommand ??= new Command(OnScrollLeftCommand);

        #endregion

        #region --Private methods--

        private void OnScrollRightCommand()
        {
            if (_firstVisibleItemIndex > 1)
            {
                collectionView.ScrollTo(_firstVisibleItemIndex - 2, -1, ScrollToPosition.Start, true);
            }
        }

        private void OnScrollLeftCommand()
        {
            if (ItemsSource.Count() - _firstVisibleItemIndex - _countVisibleItems > 1)
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

            if (ItemsSource.Count() - _firstVisibleItemIndex - _countVisibleItems < 1)
            {
                collectionView.ScrollTo(_firstVisibleItemIndex, -1, ScrollToPosition.Start, true);
            }
        }

        #endregion
    }
}