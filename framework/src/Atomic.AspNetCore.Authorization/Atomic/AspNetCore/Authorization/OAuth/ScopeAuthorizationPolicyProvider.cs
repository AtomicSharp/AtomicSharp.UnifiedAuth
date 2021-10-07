using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atomic.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    [ExposeServices(typeof(IScopeAuthorizationPolicyProvider), IncludeDefaults = false)]
    public class ScopeAuthorizationPolicyProvider : AuthorizationPolicyProviderBase, IScopeAuthorizationPolicyProvider,
        ISingletonDependency
    {
        public ScopeAuthorizationPolicyProvider()
        {
            CachedPolicies = new Dictionary<string, AuthorizationPolicy>();
        }

        protected Dictionary<string, AuthorizationPolicy> CachedPolicies { get; }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(ScopeAuthorizeAttribute.PolicyPrefix))
            {
                var scopePolicyName = policyName.Substring(ScopeAuthorizeAttribute.PolicyPrefix.Length);
                if (!string.IsNullOrEmpty(scopePolicyName))
                {
                    var policy = CachedPolicies.GetOrAdd(scopePolicyName, scopeName =>
                    {
                        var policyBuilder = new AuthorizationPolicyBuilder(Array.Empty<string>());
                        policyBuilder.AddRequirements(new ScopeRequirement(scopeName));
                        return policyBuilder.Build();
                    });
                    return Task.FromResult(policy);
                }
            }

            return Task.FromResult((AuthorizationPolicy)null);
        }
    }
}