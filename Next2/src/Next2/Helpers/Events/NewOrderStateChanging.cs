using System;
using Next2.Enums;
using Prism.Events;

namespace Next2.Helpers.Events
{
    public class NewOrderStateChanging : PubSubEvent<ENewOrderViewState>
    {
    }
}