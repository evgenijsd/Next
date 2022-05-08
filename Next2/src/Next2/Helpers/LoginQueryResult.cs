﻿using System.Collections.Generic;

namespace Next2.Helpers
{
    public class LoginQueryResult
    {
        public string? EmployeeId { get; set; }

        public List<string>? Roles { get; set; }

        public Tokens Tokens { get; set; } = new();
    }
}
