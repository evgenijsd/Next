using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class ExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }
    }
}
