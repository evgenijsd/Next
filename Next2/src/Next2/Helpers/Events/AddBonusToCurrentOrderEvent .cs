using Next2.Models;
using Next2.Models.Bindables;
using Prism.Events;

namespace Next2.Helpers.Events
{
    public class AddBonusToCurrentOrderEvent : PubSubEvent<FullOrderBindableModel>
    {
    }
}