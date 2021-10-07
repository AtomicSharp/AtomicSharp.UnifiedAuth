using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public ScopeRequirement([NotNull] string scopeName)
        {
            ScopeName = scopeName;
        }

        public string ScopeName { get; }

        public override string ToString()
        {
            return $"ScopeRequirement: {ScopeName}";
        }
    }
}