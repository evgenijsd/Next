using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API
{
    public class GenericGetExecutionResult<T> : ExecutionResult
    {
        public T? Value { get; set; }
    }
}
