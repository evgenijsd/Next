using Next2.ENums;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers
{
    public class TouchActionEventArgs : EventArgs
    {
        public TouchActionEventArgs(long id, TouchActionType type, Point location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        public long Id { get; private set; }

        public TouchActionType Type { get; private set; }

        public Point Location { get; private set; }

        public bool IsInContact { get; private set; }
    }
}
