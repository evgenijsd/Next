using Next2.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers
{
    public class BonusEvent : PubSubEvent<FullOrderBindableModel>
    {
    }
}
