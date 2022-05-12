using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GenericGetExecutionResult<T>
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public GenericGetResult<T>? Value { get; set; }
    }
}
