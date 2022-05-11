using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories
{
    public class GetSubcategoriesListQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public GetSubcategoriesListQueryResult Value { get; set; } = new();
    }
}
