namespace Next2.Helpers.DTO
{
    public class LogoutCommand
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
