namespace Next2.Helpers.DTO
{
    public class RefreshTokenQuery
    {
        public string EmployeeId { get; set; } = string.Empty;

        public Tokens Tokens { get; set; } = new();
    }
}