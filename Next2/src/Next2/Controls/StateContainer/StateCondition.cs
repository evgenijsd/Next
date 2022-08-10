using Xamarin.Forms;

namespace Next2.Controls.StateContainer
{
    [ContentProperty("Content")]
    public class StateCondition : View
    {
        public object? State { get; set; }

        public object? NotState { get; set; }

        public View Content { get; set; }
    }
}
