using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetAvailableTablesListQueryResult
    {
        public List<TableModelDTO>? Tables { get; set; } = new();
    }
}
