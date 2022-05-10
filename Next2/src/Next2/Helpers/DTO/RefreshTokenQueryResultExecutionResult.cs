﻿using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class RefreshTokenQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public RefreshTokenQueryResult Value { get; set; } = new();
    }
}
