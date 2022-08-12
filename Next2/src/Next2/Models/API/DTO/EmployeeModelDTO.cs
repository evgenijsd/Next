using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class EmployeeModelDTO
    {
        public string? EmployeeId { get; set; }
        public string? UserName { get; set; }
        public IEnumerable<OccupiedTableModelDTO>? Tables { get; set; }
    }
}
