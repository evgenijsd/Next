using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GenericGetExecutionResult<T> : ExecutionResult
    {
        public GenericGetResult<T>? Value { get; set; }
    }
}
