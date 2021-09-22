using System.Collections.Generic;

namespace Atomic.UnifiedAuth.Web.Controllers.Consent
{
    public class ConsentInputModel
    {
        public string Action { get; set; }
        public IEnumerable<string> ScopesConsented { get; init; }
        public bool RememberConsent { get; init; }
        public string ReturnUrl { get; init; }
        public string ClientDescription { get; init; }
    }
}