using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers
{
    public class MessageEvent
    {
        public static string SearchMessage => nameof(SearchMessage);

        public string SearchLine { get; }

        public MessageEvent(string searchLine)
        {
            SearchLine = searchLine;
        }
    }
}
