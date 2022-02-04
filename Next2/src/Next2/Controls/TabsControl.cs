using Next2.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    [ContentProperty("Template")]
    public class TabsControl : ContentView
    {
        private FlexLayout _flexLayout = new FlexLayout();

        private ICommand _tapCommand;
        private ICommand TapCommand => _tapCommand = new AsyncCommand<ISelectable>(OnMenuItemTapCommandAsync, allowsMultipleExecutions: false);

        public TabsControl()
        {
            _flexLayout.JustifyContent = JustifyContent;
            _flexLayout.AlignContent = AlignContent;

            Content = _flexLayout;
        }

        #region -- Public properties --

        public static readonly BindableProperty TemplateProperty = BindableProperty.Create(
            propertyName: nameof(Template),
            returnType: typeof(DataTemplate),
            declaringType: typeof(TabsControl));

        public DataTemplate Template
        {
            get => (DataTemplate)GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<ISelectable>),
            typeof(TabsControl),
            default(IEnumerable<ISelectable>),
            propertyChanged: (b, o, n) =>
            ((TabsControl)b).OnItemsSourcePropertyChanged((IEnumerable)o, (IEnumerable)n));

        public IEnumerable<ISelectable> ItemsSource
        {
            get => (IEnumerable<ISelectable>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            returnType: typeof(ISelectable),
            declaringType: typeof(TabsControl),
            default(ISelectable),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
            ((TabsControl)b).OnSelectedItemPropertyChanged((ISelectable)o, (ISelectable)n));

        public ISelectable SelectedItem
        {
            get => (ISelectable)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
            propertyName: nameof(Direction),
            returnType: typeof(FlexDirection),
            declaringType: typeof(TabsControl),
            default(FlexDirection),
            defaultBindingMode: BindingMode.TwoWay);

        public FlexDirection Direction
        {
            get => (FlexDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public static readonly BindableProperty WrapProperty = BindableProperty.Create(
            propertyName: nameof(Wrap),
            returnType: typeof(FlexWrap),
            declaringType: typeof(TabsControl),
            default(FlexWrap),
            defaultBindingMode: BindingMode.TwoWay);

        public FlexWrap Wrap
        {
            get => (FlexWrap)GetValue(WrapProperty);
            set => SetValue(WrapProperty, value);
        }

        public static readonly BindableProperty JustifyContentProperty = BindableProperty.Create(
            propertyName: nameof(JustifyContent),
            returnType: typeof(FlexJustify),
            declaringType: typeof(TabsControl),
            defaultValue: FlexJustify.SpaceBetween,
            defaultBindingMode: BindingMode.TwoWay);

        public FlexJustify JustifyContent
        {
            get => (FlexJustify)GetValue(JustifyContentProperty);
            set => SetValue(JustifyContentProperty, value);
        }

        public static readonly BindableProperty AlignContentProperty = BindableProperty.Create(
            propertyName: nameof(AlignContent),
            returnType: typeof(FlexAlignContent),
            declaringType: typeof(TabsControl),
            defaultValue: FlexAlignContent.SpaceBetween,
            defaultBindingMode: BindingMode.TwoWay);

        public FlexAlignContent AlignContent
        {
            get => (FlexAlignContent)GetValue(AlignContentProperty);
            set => SetValue(AlignContentProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Direction):
                    _flexLayout.Direction = Direction;
                    break;
                case nameof(Wrap):
                    _flexLayout.Wrap = Wrap;
                    break;
                case nameof(JustifyContent):
                    _flexLayout.JustifyContent = JustifyContent;
                    break;
                case nameof(AlignContent):
                    _flexLayout.AlignContent = AlignContent;
                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private void OnItemsSourcePropertyChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            SelectedItem = null;

            _flexLayout.Children.Clear();

            CreateMenuItems();
        }

        private void OnSelectedItemPropertyChanged(ISelectable oldSelected, ISelectable newSelected)
        {
            UnselectItem();

            if (newSelected != null)
            {
                SelectItem(newSelected);
            }
        }

        private void CreateMenuItems()
        {
            if (ItemsSource != null)
            {
                foreach (ISelectable item in ItemsSource)
                {
                    if (item.IsSelected)
                    {
                        SelectedItem = item;
                    }

                    var clickableLayout = new StackLayout();

                    clickableLayout.BindingContext = item;
                    clickableLayout.Children.Add((View)Template.CreateContent());

                    clickableLayout.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = TapCommand,
                        CommandParameter = item,
                    });

                    _flexLayout.Children.Add(clickableLayout);
                }
            }
        }

        private Task OnMenuItemTapCommandAsync(ISelectable item)
        {
            if (SelectedItem != null)
            {
                UnselectItem();
            }

            SelectItem(item);

            return Task.CompletedTask;
        }

        private void SelectItem(ISelectable item)
        {
            foreach (View child in _flexLayout.Children)
            {
                if (child.BindingContext is ISelectable itemMenu && itemMenu == item)
                {
                    itemMenu.IsSelected = true;
                    SelectedItem = itemMenu;

                    break;
                }
            }
        }

        private void UnselectItem()
        {
            foreach (View child in _flexLayout.Children)
            {
                if (child.BindingContext is ISelectable itemMenu && itemMenu.IsSelected)
                {
                    itemMenu.IsSelected = false;
                    break;
                }
            }
        }

        #endregion
    }
}