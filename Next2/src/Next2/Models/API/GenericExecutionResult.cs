﻿using System.Collections.Generic;

namespace Next2.Models.API
{
    public class GenericExecutionResult<T> : ExecutionResult
    {
        public T? Value { get; set; }
    }
}