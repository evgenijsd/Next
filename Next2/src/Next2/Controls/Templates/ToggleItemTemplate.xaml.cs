﻿using Next2.Interfaces;
using Next2.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class ToggleItemTemplate : StackLayout
    {
        public ToggleItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty BindableLayoutProperty = BindableProperty.Create(
            propertyName: nameof(BindableLayout),
            returnType: typeof(IBaseModel),
            declaringType: typeof(ToggleItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public IBaseModel? BindableLayout
        {
            get => (IBaseModel)GetValue(BindableLayoutProperty);
            set => SetValue(BindableLayoutProperty, value);
        }

        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            propertyName: nameof(State),
            returnType: typeof(IBaseModel),
            declaringType: typeof(ToggleItemTemplate));

        public IBaseModel? State
        {
            get => (IBaseModel)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public static readonly BindableProperty IsToggleProperty = BindableProperty.Create(
            propertyName: nameof(IsToggle),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate));

        public bool IsToggle
        {
            get => (bool)GetValue(IsToggleProperty);
            set => SetValue(IsToggleProperty, value);
        }

        public static readonly BindableProperty CanTurnOffProperty = BindableProperty.Create(
            propertyName: nameof(CanTurnOff),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate));

        public bool CanTurnOff
        {
            get => (bool)GetValue(CanTurnOffProperty);
            set => SetValue(CanTurnOffProperty, value);
        }

        public static readonly BindableProperty IsVisibleSubtitleProperty = BindableProperty.Create(
            propertyName: nameof(IsVisibleSubtitle),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate));

        public bool IsVisibleSubtitle
        {
            get => (bool)GetValue(IsVisibleSubtitleProperty);
            set => SetValue(IsVisibleSubtitleProperty, value);
        }

        public static readonly BindableProperty IsVisibleImageProperty = BindableProperty.Create(
            propertyName: nameof(IsVisibleImage),
            returnType: typeof(bool),
            declaringType: typeof(ToggleItemTemplate));

        public bool IsVisibleImage
        {
            get => (bool)GetValue(IsVisibleImageProperty);
            set => SetValue(IsVisibleImageProperty, value);
        }

        public static readonly BindableProperty ImagePathProperty = BindableProperty.Create(
            propertyName: nameof(ImagePath),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate));

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(
            propertyName: nameof(Subtitle),
            returnType: typeof(string),
            declaringType: typeof(ToggleItemTemplate));

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(State):
                    IsToggle = State?.Id == BindableLayout?.Id;

                    break;
                case nameof(BindableLayout):
                    IsToggle = State?.Id == BindableLayout?.Id;

                    break;
                case nameof(IsToggle):
                    if (IsToggle)
                    {
                        BindableLayout = State;
                    }

                    break;
            }
        }

        #endregion
    }
}