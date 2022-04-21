﻿using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class GuardEntry : StackLayout
    {
        public GuardEntry()
        {
            InitializeComponent();
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

        public static readonly BindableProperty BehaviorProperty = BindableProperty.Create(
            propertyName: nameof(Behavior),
            returnType: typeof(Behavior<HideClipboardEntry>),
            declaringType: typeof(GuardEntry),
            defaultBindingMode: BindingMode.TwoWay);

        public Behavior<HideClipboardEntry> Behavior
        {
            get => (Behavior<HideClipboardEntry>)GetValue(BehaviorProperty);
            set => SetValue(BehaviorProperty, value);
        }

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(Behavior) && Behavior is not null)
            {
                entry.Behaviors.Add(Behavior);
            }
        }
    }
}