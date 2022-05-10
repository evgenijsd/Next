using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GetCustomersListQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public GetCustomersListQueryResult Value { get; set; } = new();
    }
}
