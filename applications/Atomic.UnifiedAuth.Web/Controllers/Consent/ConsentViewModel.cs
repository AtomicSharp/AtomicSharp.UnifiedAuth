using System.Collections.Generic;

namespace Atomic.UnifiedAuth.Web.Controllers.Consent
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string ClientName { get; init; }
        public string ClientUrl { get; init; }
        public string ClientLogoUrl { get; init; }
        public bool AllowRememberConsent { get; init; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
    }
}