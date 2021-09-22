namespace Atomic.UnifiedAuth.Web.Controllers.Consent
{
    public class ScopeViewModel
    {
        public string Value { get; init; }
        public string DisplayName { get; init; }
        public string Description { get; init; }
        public bool Emphasize { get; init; }
        public bool Required { get; init; }
        public bool Checked { get; init; }
    }
}