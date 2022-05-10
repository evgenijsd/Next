using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class GenericExecutionResult<T> : ExecutionResult
    {
        public T? Value { get; set; }
    }
}
