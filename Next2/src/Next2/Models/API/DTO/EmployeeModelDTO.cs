using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class EmployeeModelDTO
    {
        public string? EmployeeId { get; set; }

        public string? UserName { get; set; }

        public IEnumerable<OccupiedTableModelDTO>? Tables { get; set; }
    }
}
