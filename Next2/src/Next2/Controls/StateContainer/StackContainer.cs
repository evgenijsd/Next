using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Next2.Controls.StateContainer
{
    [ContentProperty("Conditions")]
    public class StackContainer : StackLayout
    {
        #region -- Public properties --

        public List<StackContainerCondition> Conditions { get; set; } = new List<StackContainerCondition>();

        public static readonly BindableProperty CurrentStateProperty = BindableProperty.Create(
            propertyName: nameof(CurrentState),
            returnType: typeof(object),
            declaringType: typeof(StateContainer),
            propertyChanged: OnCurrentStatePropertyChanged);

        public object CurrentState
        {
            get => GetValue(CurrentStateProperty);
            set => SetValue(CurrentStateProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "Renderer")
            {
                Children.Clear();

                var currentState = CurrentState?.ToString();

                foreach (var condition in Conditions)
                {
                    condition.IsVisible = condition.State?.ToString() == currentState;
                    condition.VerticalOptions = LayoutOptions.FillAndExpand;
                    condition.HorizontalOptions = LayoutOptions.FillAndExpand;

                    Children.Add(condition);
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private static void OnCurrentStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StackContainer parent)
            {
                parent.ChooseStateAsync(newValue);
            }
        }

        private Task ChooseStateAsync(object state)
        {
            foreach (var condition in Conditions)
            {
                condition.IsVisible = false;
            }

            var stringState = state?.ToString();
            var currentCondition = Conditions.FirstOrDefault(x => x?.State?.ToString() == stringState);

            if (currentCondition is null)
            {
                currentCondition = Conditions.FirstOrDefault(x => x?.NotState?.ToString() != stringState);
            }

            if (currentCondition is not null)
            {
                currentCondition.IsVisible = true;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
