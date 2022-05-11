using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories.GetSubcategoryById
{
    public class GetSubcategoryByIdQueryResultExecutionResult
    {
        public bool Success { get; set; }

        public List<ErrorInfo>? Errors { get; set; }

        public List<InfoMessage>? Messages { get; set; }

        public GetSubcategoryByIdQueryResult Value { get; set; } = new();
    }
}
