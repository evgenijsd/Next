namespace Next2.Models.API.Results
{
    public class RefreshTokenQueryResult
    {
        public string? EmployeeId { get; set; }

        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public Tokens Tokens { get; set; } = new();
    }
}
