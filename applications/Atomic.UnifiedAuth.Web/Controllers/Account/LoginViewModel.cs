using System.Collections.Generic;
using System.Linq;

namespace Atomic.UnifiedAuth.Web.Controllers.Account
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; init; } = true;

        public bool EnableLocalLogin { get; init; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly =>
            EnableLocalLogin == false && ExternalProviders.Any();

        public string ExternalLoginScheme =>
            IsExternalLoginOnly
                ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme
                : null;
    }
}