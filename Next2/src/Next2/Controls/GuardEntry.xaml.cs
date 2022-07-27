using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class GuardEntry : StackLayout
    {
        public GuardEntry()
        {
            InitializeComponent();

            EntryBehaviors = new();
        }

        #region -- Public properties --

        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
            propertyName: nameof(Keyboard),
            returnType: typeof(Keyboard),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
            propertyName: nameof(MaxLength),
            returnType: typeof(int),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(float),
            defaultValue: 3.0f,
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(
            propertyName: nameof(TextSize),
            returnType: typeof(double),
            defaultValue: 14.0d,
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(
            propertyName: nameof(ErrorText),
            returnType: typeof(string),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public static readonly BindableProperty IsValidProperty = BindableProperty.Create(
            propertyName: nameof(IsValid),
            returnType: typeof(bool),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public static readonly BindableProperty EntryBehaviorsProperty = BindableProperty.Create(
            nameof(EntryBehaviors),
            typeof(ObservableCollection<Behavior<NoActionMenuEntry>>),
            typeof(GuardEntry),
            propertyChanged: (b, o, n) =>
            ((GuardEntry)b).OnItemsSourcePropertyChanged((IEnumerable)o, (IEnumerable)n));

        public ObservableCollection<Behavior<NoActionMenuEntry>> EntryBehaviors
        {
            get => (ObservableCollection<Behavior<NoActionMenuEntry>>)GetValue(EntryBehaviorsProperty);
            set => SetValue(EntryBehaviorsProperty, value);
        }

        #endregion

        #region -- Private helpers --

        private void OnItemsSourcePropertyChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            if (oldItemsSource is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            if (newItemsSource is INotifyCollectionChanged ncc1)
            {
                ncc1.CollectionChanged += OnItemsSourceCollectionChanged;
            }

            entry.Behaviors.Clear();

            AddBehaviorItems(EntryBehaviors);
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex == -1)
                    {
                        entry.Behaviors.Clear();
                    }

                    AddBehaviorItems(e.NewItems);

                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex == -1 || e.NewStartingIndex == -1)
                    {
                        entry.Behaviors.Clear();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex == -1)
                    {
                        entry.Behaviors.Clear();
                    }

                    RemoveBehaviorItems(e.OldItems);

                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex == -1)
                    {
                        entry.Behaviors.Clear();
                    }

                    RemoveBehaviorItems(e.OldItems);
                    AddBehaviorItems(e.NewItems);

                    break;
                case NotifyCollectionChangedAction.Reset:
                    entry.Behaviors.Clear();
                    break;
            }
        }

        private void RemoveBehaviorItems(IEnumerable items)
        {
            foreach (object item in items)
            {
                if (item is Behavior<NoActionMenuEntry> behavior)
                {
                    entry.Behaviors.Remove(behavior);
                }
            }
        }

        private void AddBehaviorItems(IEnumerable items)
        {
            foreach (object item in items)
            {
                if (item is Behavior<NoActionMenuEntry> behavior)
                {
                    entry.Behaviors.Add(behavior);
                }
            }
        }

        #endregion
    }
}