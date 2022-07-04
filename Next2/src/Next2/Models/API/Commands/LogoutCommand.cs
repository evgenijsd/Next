namespace Next2.Models.API.Commands
{
    public class LogoutCommand
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
