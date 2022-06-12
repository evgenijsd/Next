using Prism.Events;
using System;

namespace Next2.Helpers.Events
{
    public class OrderSelectedEvent : PubSubEvent<Guid>
    {
    }
}
