using Atomic.AspNetCore.Authorization;
using Atomic.AspNetCore.Authorization.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ServiceCollectionAuthorizationExtensionsTest
    {
        [Fact]
        public void Should_Auto_Register_Services()
        {
            var services = new ServiceCollection();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            services.ShouldContainSingleton(
                typeof(IAuthorizationHandler),
                typeof(ScopeRequirementHandler)
            );
            services.ShouldContainSingleton(
                typeof(IAuthorizationPolicyProvider),
                typeof(AtomicAuthorizationPolicyProvider)
            );
            services.ShouldContainSingleton(
                typeof(IScopeAuthorizationPolicyProvider),
                typeof(ScopeAuthorizationPolicyProvider)
            );
        }

        [Fact]
        public void Should_Add_Scope_Policy_Provider()
        {
            var services = new ServiceCollection();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            var sp = services.BuildServiceProvider();
            var options = sp.GetRequiredService<IOptions<AtomicAuthorizationOptions>>().Value;
            options.AuthorizationPolicyProviders.Count.ShouldBe(1);
            options.AuthorizationPolicyProviders[0].ShouldBe(typeof(IScopeAuthorizationPolicyProvider));
        }
    }
}