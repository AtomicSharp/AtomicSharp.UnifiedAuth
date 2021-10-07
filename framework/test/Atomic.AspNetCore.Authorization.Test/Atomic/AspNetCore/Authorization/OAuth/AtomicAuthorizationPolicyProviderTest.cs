using Microsoft.AspNetCore.Authorization;
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

            var policy = await provider.GetPolicyAsync("Scope:Author.Get");
            policy.ShouldNotBeNull();
            policy.Requirements.Count.ShouldBe(1);
            var requirement = policy.Requirements[0];
            requirement.ShouldBeOfType<ScopeRequirement>();
            ((ScopeRequirement)requirement).ScopeName.ShouldBe("Author.Get");
        }
    }
}