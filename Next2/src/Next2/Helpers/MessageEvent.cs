using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers
{
    public class MessageEvent
    {
        public static string SearchMessage => nameof(SearchMessage);

        public string Search { get; }

        public MessageEvent(string search)
        {
            Search = search;
        }
    }
}
