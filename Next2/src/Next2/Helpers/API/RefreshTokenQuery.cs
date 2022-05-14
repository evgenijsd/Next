namespace Next2.Helpers.API
{
    public class RefreshTokenQuery
    {
        public string EmployeeId { get; set; } = string.Empty;

        public Tokens Tokens { get; set; } = new();
    }
}