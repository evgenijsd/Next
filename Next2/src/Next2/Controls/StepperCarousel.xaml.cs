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
        public TouchInteractionStatus InteractionStatusTabsControl
        {
            get;
            set;
        }

        public ICommand TapLeftButtonCommand => new Command(OnToachStartedLeftButtonCommand);

        public ICommand TapRightButtonCommand => new Command(OnToachStartedRightButtonCommand);

        public ICommand TapTabsControlCommand => new Command(OnToachStartedTabsControlCommand);

        public ICommand TapTabsControlCommand2 => new Command(OnToachStartedTabsControlCommand2);

        public ICommand ScrolledCommand => new Command(OnScrolledCommand);

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
        }

        private void OnToachStartedRightButtonCommand(object obj)
        {
        }

        private void OnToachStartedTabsControlCommand2(object obj)
        {
            Console.WriteLine($"ScrollView = {obj.ToString()}, {count++}");
        }

        private void OnToachStartedTabsControlCommand(object obj)
        {
            Console.WriteLine($"Button = {obj.ToString()}, {count++}");
        }

        private void OnScrolledCommand(object obj)
        {
            //Console.WriteLine($"Scrolled = {obj.ToString()}, {count++}");
        }

        //private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        //{
        //    if (scrollCategories.Children.FirstOrDefault() is TabsControl tabs)
        //    {
        //        if (tabs.Content is FlexLayout flexLayout)
        //        {
        //            if (flexLayout.Children.FirstOrDefault() is StackLayout child)
        //            {
        //                var scrollX = scrollCategories.ScrollX;

        //                var widthChild = child.Width;

        //                var childsLeft = (flexLayout.Width - scrollX) / widthChild;

        //                if (childsLeft > 4)
        //                {
        //                    var scrolledChilds = (int)(scrollX / widthChild);

        //                    scrollCategories.ScrollToAsync((scrolledChilds + 1) * widthChild, 0, true);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void TapGestureRecognizer_Tapped_1(object sender, System.EventArgs e)
        //{
        //    if (scrollCategories.Children.FirstOrDefault() is TabsControl tabs)
        //    {
        //        if (tabs.Content is FlexLayout flexLayout)
        //        {
        //            if (flexLayout.Children.FirstOrDefault() is StackLayout child)
        //            {
        //                var scrollX = scrollCategories.ScrollX;

        //                var widthChild = child.Width;

        //                if (scrollX >= widthChild)
        //                {
        //                    var scrolledChilds = (int)(scrollX / widthChild);

        //                    scrollCategories.ScrollToAsync((scrolledChilds - 1) * widthChild, 0, true);
        //                }
        //            }
        //        }
        //    }
        //}
        private int oldScrolled = 0;

        private void scrollCategories_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (sender is ScrollView scroll)
            {
                if (scroll.Children.FirstOrDefault() is TabsControl tabs)
                {
                    var x = (int)scrollCategories.ScrollX;
                    //Console.WriteLine($"Scrolled = {oldScrolled.ToString()}, {x.ToString()}, {count++}");
                    oldScrolled = x;
                    //Task.Run(() => MethodWithParameter(scroll, tabs, x));
                    Device.StartTimer(TimeSpan.FromSeconds(1), () => MethodWithParameter(scroll, tabs, x));
                }
            }
        }

        private bool MethodWithParameter(ScrollView scroll, TabsControl tabs, int x)
        {
            bool result = false;

            if (oldScrolled == x)
            {
                if (tabs.Content is FlexLayout flexLayout)
                {
                    if (flexLayout.Children.FirstOrDefault() is StackLayout child)
                    {
                        var width = flexLayout.Width;

                        var widthChild = child.Width;
                        int countItemScrolled = (int)x / (int)widthChild;

                        var offset = x - (countItemScrolled * widthChild);

                        if (offset > 0)
                        {
                            if (widthChild / 2 < offset)
                            {
                                scroll.ScrollToAsync(x + (widthChild - offset), 0, true);
                            }
                            else
                            {
                                scroll.ScrollToAsync(x - offset, 0, true);
                            }
                        }
                    }
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        #endregion

        private int count = 0;

        private void TouchEffect_TouchAction(object sender, Helpers.TouchActionEventArgs args)
        {
            //var sss = sender as CategoryItemTemplate;
            //sss.IsSelected = true;
            //Console.WriteLine($"Button = {args.Type.ToString()}, {count++}");
        }

        private void TouchEffect_TouchAction2(object sender, Helpers.TouchActionEventArgs args)
        {
            //var sss = sender as CategoryItemTemplate;
            //sss.IsSelected = true;
            //Console.WriteLine($"ScrollView = {args.Type.ToString()}, {args.Location.ToString()}, {count++}");
        }
    }
}