using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    /// <summary>
    /// Authorize the resource with OAuth scope from jwt
    /// </summary>
    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefix = "Scope:";

        public ScopeAuthorizeAttribute([NotNull] string scopeName)
        {
            ScopeName = scopeName;
        }

        public string ScopeName
        {
            get => Policy!.Substring(PolicyPrefix.Length);
            set => Policy = $"{PolicyPrefix}{value}";
        }
    }
}