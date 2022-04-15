using Next2.Enums;
using System;
using Xamarin.Forms;

namespace Next2.Effects
{
    public class TouchActionEventArgs : EventArgs
    {
        public TouchActionEventArgs(long id, ETouchActionType type, Point location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        public long Id { get; private set; }

        public ETouchActionType Type { get; private set; }

        public Point Location { get; private set; }

        public bool IsInContact { get; private set; }
    }
}
