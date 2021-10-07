using Atomic.AspNetCore.Authorization;
using Atomic.AspNetCore.Authorization.OAuth;
using Microsoft.AspNetCore.Authorization;
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
    }
}