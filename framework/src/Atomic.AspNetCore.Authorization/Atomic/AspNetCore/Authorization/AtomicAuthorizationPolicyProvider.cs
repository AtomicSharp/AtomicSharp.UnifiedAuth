using System.Threading.Tasks;
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
            IOptions<AtomicAuthorizationOptions> atomicAuthorizationOptions,
            ILazyServiceProvider lazyServiceProvider
        )
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            LazyServiceProvider = lazyServiceProvider;
            AtomicAuthorizationOptions = atomicAuthorizationOptions.Value;
        }

        protected DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        protected ILazyServiceProvider LazyServiceProvider { get; }

        protected AtomicAuthorizationOptions AtomicAuthorizationOptions { get; }

        public virtual async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var providerType in AtomicAuthorizationOptions.AuthorizationPolicyProviders)
            {
                var provider = (IAuthorizationPolicyProvider)LazyServiceProvider.LazyGetRequiredService(providerType);
                var policy = await provider.GetPolicyAsync(policyName);
                if (policy != null) return policy;
            }

            return await FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public virtual async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            foreach (var providerType in AtomicAuthorizationOptions.AuthorizationPolicyProviders)
            {
                var provider = (IAuthorizationPolicyProvider)LazyServiceProvider.LazyGetRequiredService(providerType);
                var policy = await provider.GetDefaultPolicyAsync();
                if (policy != null) return policy;
            }

            return await FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public virtual async Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            foreach (var providerType in AtomicAuthorizationOptions.AuthorizationPolicyProviders)
            {
                var provider = (IAuthorizationPolicyProvider)LazyServiceProvider.LazyGetRequiredService(providerType);
                var policy = await provider.GetFallbackPolicyAsync();
                if (policy != null) return policy;
            }

            return await FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}