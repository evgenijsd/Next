using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Tables.GetListAvailableTables
{
    public class GetAvailableTablesListQueryResult
    {
        public List<TableModelDTO>? Tables { get; set; } = new();
    }
}
