using Atomic.Collections;
using Microsoft.AspNetCore.Authorization;

namespace Atomic.AspNetCore.Authorization
{
    public class AtomicAuthorizationOptions
    {
        public AtomicAuthorizationOptions()
        {
            AuthorizationPolicyProviders = new TypeList<IAuthorizationPolicyProvider>();
        }

        public ITypeList<IAuthorizationPolicyProvider> AuthorizationPolicyProviders { get; }
    }
}