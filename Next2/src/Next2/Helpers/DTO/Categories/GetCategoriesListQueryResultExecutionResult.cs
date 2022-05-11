using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Categories
{
    public class GetCategoriesListQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public GetCategoriesListQueryResult Value { get; set; } = new();
    }
}
