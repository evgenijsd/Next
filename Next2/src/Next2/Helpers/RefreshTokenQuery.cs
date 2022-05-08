using System.Collections.Generic;

namespace Next2.Helpers
{
    public class RefreshTokenQuery
    {
        public string EmployeeId { get; set; } = string.Empty;

        public Tokens Tokens { get; set; } = new();
    }
}