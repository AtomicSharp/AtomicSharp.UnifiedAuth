using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    public class AtomicAuthorizationPolicyProviderTest
    {
        [Fact]
        public async void Should_Get_Policy()
        {
            var services = new ServiceCollection();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            var sp = services.BuildServiceProvider();
            var provider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

            // should get policy from scope policy provider
            var policy = await provider.GetPolicyAsync("Scope:Author.Get");
            policy.ShouldNotBeNull();
            policy.Requirements.Count.ShouldBe(1);
            var requirement = policy.Requirements[0];
            requirement.ShouldBeOfType<ScopeRequirement>();
            ((ScopeRequirement)requirement).ScopeName.ShouldBe("Author.Get");

            // should return null
            var nonPolicy = await provider.GetPolicyAsync("Non_Policy_Name");
            nonPolicy.ShouldBeNull();
        }

        [Fact]
        public async void Should_Get_Default_Policy()
        {
            var services = new ServiceCollection();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            var sp = services.BuildServiceProvider();
            var provider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

            var defaultPolicy = await provider.GetDefaultPolicyAsync();
            defaultPolicy.ShouldNotBeNull();
            defaultPolicy.Requirements.Count.ShouldBe(1);
            defaultPolicy.Requirements[0].ShouldBeOfType<DenyAnonymousAuthorizationRequirement>();
        }

        [Fact]
        public async void Should_Get_Non_Fallback_Policy()
        {
            var services = new ServiceCollection();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            var sp = services.BuildServiceProvider();
            var provider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

            var fallbackPolicy = await provider.GetFallbackPolicyAsync();
            fallbackPolicy.ShouldBeNull();
        }

        [Fact]
        public async void Should_Get_Fallback_Policy_If_Configured()
        {
            var services = new ServiceCollection();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            var newFallbackPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new DenyAnonymousAuthorizationRequirement())
                .Build();
            services.Configure<AuthorizationOptions>(options =>
            {
                options.FallbackPolicy = newFallbackPolicy;
            });

            var sp = services.BuildServiceProvider();
            var provider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

            var fallbackPolicy = await provider.GetFallbackPolicyAsync();
            fallbackPolicy.ShouldBe(newFallbackPolicy);
        }
    }
}