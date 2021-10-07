using System.Linq;
using System.Threading.Tasks;
using Atomic.Extensions.DependencyInjection;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    [ExposeServices(typeof(IAuthorizationHandler))]
    public class ScopeRequirementHandler : AuthorizationHandler<ScopeRequirement>, ISingletonDependency
    {
        private readonly ILogger<ScopeRequirementHandler> _logger;

        public ScopeRequirementHandler(ILogger<ScopeRequirementHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ScopeRequirement requirement
        )
        {
            _logger.LogWarning("Evaluating authorization requirement for {requirement}", requirement);

            var scopeClaim = context.User.FindFirst(c => c.Type == JwtClaimTypes.Scope);
            if (scopeClaim != null)
            {
                var scopes = scopeClaim.Value.Split(' ');
                if (scopes.Contains(requirement.ScopeName))
                {
                    _logger.LogInformation("Authorization requirement for {requirement} satisfied", requirement);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogInformation(
                        "Authorization requirement for {requirement} can not be satisfied, candidate scopes are {availableScopes}",
                        requirement, scopes);
                }
            }
            else
            {
                _logger.LogInformation("No Scope claim present");
            }

            return Task.CompletedTask;
        }
    }
}