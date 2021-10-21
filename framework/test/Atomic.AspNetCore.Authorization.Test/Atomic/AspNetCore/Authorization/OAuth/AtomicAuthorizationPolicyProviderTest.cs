using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public static AuthorizationPolicy Policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new DenyAnonymousAuthorizationRequirement())
            .Build();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(Policy);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(Policy);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult(Policy);
        }
    }

    public class AtomicAuthorizationPolicyProviderTest
    {
        private IAuthorizationPolicyProvider _policyProvider;
        private ServiceCollection _services;

        public AtomicAuthorizationPolicyProviderTest()
        {
            _services = new ServiceCollection();
            _services.AddAtomicDependencyInjection();
            _services.AddAtomicAuthorization();
            _policyProvider = _services.BuildServiceProvider().GetRequiredService<IAuthorizationPolicyProvider>();
        }

        [Fact]
        public async void Should_Get_Policy()
        {
            // should get policy from scope policy provider
            var policy = await _policyProvider.GetPolicyAsync("Scope:Author.Get");
            policy.ShouldNotBeNull();
            policy.Requirements.Count.ShouldBe(1);
            var requirement = policy.Requirements[0];
            requirement.ShouldBeOfType<ScopeRequirement>();
            ((ScopeRequirement)requirement).ScopeName.ShouldBe("Author.Get");

            // should return null
            var nonPolicy = await _policyProvider.GetPolicyAsync("Non_Policy_Name");
            nonPolicy.ShouldBeNull();
        }

        [Fact]
        public async void Should_Get_Default_Policy()
        {
            var defaultPolicy = await _policyProvider.GetDefaultPolicyAsync();
            defaultPolicy.ShouldNotBeNull();
            defaultPolicy.Requirements.Count.ShouldBe(1);
            defaultPolicy.Requirements[0].ShouldBeOfType<DenyAnonymousAuthorizationRequirement>();
        }

        [Fact]
        public async void Should_Get_Non_Fallback_Policy()
        {
            var fallbackPolicy = await _policyProvider.GetFallbackPolicyAsync();
            fallbackPolicy.ShouldBeNull();
        }

        [Fact]
        public async void Should_Get_Fallback_Policy_If_Configured()
        {
            var newFallbackPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new DenyAnonymousAuthorizationRequirement())
                .Build();

            _services.Configure<AuthorizationOptions>(options =>
            {
                options.FallbackPolicy = newFallbackPolicy;
            });

            // since we modified the option, a new policy provider should be used to update the option
            var provider = _services.BuildServiceProvider().GetRequiredService<IAuthorizationPolicyProvider>();

            var fallbackPolicy = await provider.GetFallbackPolicyAsync();
            fallbackPolicy.ShouldBe(newFallbackPolicy);
        }

        [Fact]
        public async void Should_Get_Policy_From_Custom_Provider()
        {
            _services.AddSingleton<CustomAuthorizationPolicyProvider>();
            _services.Configure<AtomicAuthorizationOptions>(options =>
            {
                options.AuthorizationPolicyProviders.TryAdd<CustomAuthorizationPolicyProvider>();
            });

            var provider = _services.BuildServiceProvider().GetRequiredService<IAuthorizationPolicyProvider>();

            var policy = await provider.GetPolicyAsync("fallback_to_custom");
            policy.ShouldBe(CustomAuthorizationPolicyProvider.Policy);

            var fallbackPolicy = await provider.GetFallbackPolicyAsync();
            fallbackPolicy.ShouldBe(CustomAuthorizationPolicyProvider.Policy);

            var defaultPolicy = await provider.GetDefaultPolicyAsync();
            defaultPolicy.ShouldBe(CustomAuthorizationPolicyProvider.Policy);
        }
    }
}