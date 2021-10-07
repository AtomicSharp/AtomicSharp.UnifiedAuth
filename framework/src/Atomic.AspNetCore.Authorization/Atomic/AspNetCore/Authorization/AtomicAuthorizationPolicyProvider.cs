using System.Threading.Tasks;
using Atomic.AspNetCore.Authorization.OAuth;
using Atomic.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Atomic.AspNetCore.Authorization
{
    [Dependency(ReplaceServices = true)]
    public class AtomicAuthorizationPolicyProvider : IAuthorizationPolicyProvider, ISingletonDependency
    {
        public AtomicAuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options,
            ILazyServiceProvider lazyServiceProvider
        )
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            LazyServiceProvider = lazyServiceProvider;
        }

        protected DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        protected ILazyServiceProvider LazyServiceProvider { get; }

        protected IScopeAuthorizationPolicyProvider ScopePolicyProvider =>
            LazyServiceProvider.LazyGetRequiredService<IScopeAuthorizationPolicyProvider>();

        public virtual async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return await ScopePolicyProvider.GetPolicyAsync(policyName)
                   ?? await FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public virtual Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
            => FallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}