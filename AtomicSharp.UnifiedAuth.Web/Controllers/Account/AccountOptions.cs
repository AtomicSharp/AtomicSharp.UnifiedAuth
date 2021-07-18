namespace AtomicSharp.UnifiedAuth.Web.Controllers.Account
{
    public class AccountOptions
    {
        public bool AllowLocalLogin { get; set; }
        public bool AllowRememberLogin { get; set; }
        public bool AutomaticRedirectAfterSignOut { get; set; }
    }
}