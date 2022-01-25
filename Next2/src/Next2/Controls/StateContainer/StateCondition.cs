using Xamarin.Forms;

namespace MapNotePad.Controls.StateContainer
{
    [ContentProperty("Content")]
    public class StateCondition : View
    {
        public object State { get; set; }
        public View Content { get; set; }
    }
}