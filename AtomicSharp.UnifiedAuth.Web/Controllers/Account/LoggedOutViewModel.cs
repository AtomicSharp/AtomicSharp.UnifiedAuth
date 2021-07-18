namespace AtomicSharp.UnifiedAuth.Web.Controllers.Account
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; init; }
        public string ClientName { get; init; }
        public string SignOutIframeUrl { get; init; }
        public bool AutomaticRedirectAfterSignOut { get; init; }
        public string ExternalAuthenticationScheme { get; set; }
        public string LogoutId { get; set; }
        public bool TriggerExternalSignOut => ExternalAuthenticationScheme != null;
    }
}