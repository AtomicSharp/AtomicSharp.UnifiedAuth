using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Atomic.AspNetCore.Authorization
{
    public abstract class AuthorizationPolicyProviderBase : IAuthorizationPolicyProvider
    {
        public abstract Task<AuthorizationPolicy> GetPolicyAsync(string policyName);

        public virtual Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => Task.FromResult((AuthorizationPolicy)null);

        public virtual Task<AuthorizationPolicy> GetFallbackPolicyAsync()
            => Task.FromResult((AuthorizationPolicy)null);
    }
}