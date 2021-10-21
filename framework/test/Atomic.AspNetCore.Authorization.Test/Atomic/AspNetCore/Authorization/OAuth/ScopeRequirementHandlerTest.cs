using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    public class ScopeRequirementHandlerTest
    {
        private ServiceProvider _serviceProvider;

        public ScopeRequirementHandlerTest()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAtomicDependencyInjection();
            services.AddAtomicAuthorization();

            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("Author.Get", builder =>
                {
                    builder.AddRequirements(new ScopeRequirement("Author.Get"));
                });
                options.AddPolicy("Author.Create", builder =>
                {
                    builder.AddRequirements(new ScopeRequirement("Author.Create"));
                });
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Should_Handle_Scope_Requirement_Success()
        {
            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Scope, "Author.Get")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();

            var result = await authService.AuthorizeAsync(claimsPrincipal, "Author.Get");
            result.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task Should_Handle_Scope_Requirement_Fail_For_Non_Scope()
        {
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();

            var result = await authService.AuthorizeAsync(claimsPrincipal, "Author.Create");
            result.Succeeded.ShouldBe(false);
            result.Failure.ShouldNotBeNull();
            result.Failure.FailedRequirements.Count().ShouldBe(1);
            var requirement = result.Failure.FailedRequirements.First();
            requirement.ShouldBeAssignableTo<ScopeRequirement>();
            var failedRequirement = (ScopeRequirement)requirement;
            failedRequirement.ScopeName.ShouldBe("Author.Create");
        }

        [Fact]
        public async Task Should_Handle_Scope_Requirement_Fail_For_Lack_Of_Scope()
        {
            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Scope, "Author.Get")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();

            var result = await authService.AuthorizeAsync(claimsPrincipal, "Author.Create");
            result.Succeeded.ShouldBe(false);
            result.Failure.ShouldNotBeNull();
            result.Failure.FailedRequirements.Count().ShouldBe(1);
            var requirement = result.Failure.FailedRequirements.First();
            requirement.ShouldBeAssignableTo<ScopeRequirement>();
            var failedRequirement = (ScopeRequirement)requirement;
            failedRequirement.ScopeName.ShouldBe("Author.Create");
        }
    }
}