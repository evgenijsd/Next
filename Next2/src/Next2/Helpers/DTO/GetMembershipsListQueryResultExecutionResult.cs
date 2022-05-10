using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GetMembershipsListQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public GetMembershipsListQueryResult Value { get; set; } = new();
    }
}
