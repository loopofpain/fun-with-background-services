namespace TheWindowsService.Test2.AuthenticationHandler
{
    public class ApiAuthenticationOptions
    {
        public string Grant { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UrlToService { get; set; }
        public string UrlToAuthenticationProvider { get; set; }
    }
}