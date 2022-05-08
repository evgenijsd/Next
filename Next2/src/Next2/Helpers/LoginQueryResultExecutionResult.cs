using System.Collections.Generic;

namespace Next2.Helpers
{
    public class LoginQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public LoginQueryResult Value { get; set; } = new();
    }
}
