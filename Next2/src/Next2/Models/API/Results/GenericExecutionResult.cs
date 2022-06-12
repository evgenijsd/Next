namespace Next2.Models.API.Results
{
    public class GenericExecutionResult<T> : ExecutionResult
    {
        public T? Value { get; set; }
    }
}