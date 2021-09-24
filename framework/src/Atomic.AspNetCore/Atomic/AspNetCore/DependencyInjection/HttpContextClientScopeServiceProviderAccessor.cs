using System;
using Atomic.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Atomic.AspNetCore.DependencyInjection
{
    public class HttpContextClientScopeServiceProviderAccessor : IClientScopeServiceProviderAccessor,
        ISingletonDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextClientScopeServiceProviderAccessor(
            IHttpContextAccessor httpContextAccessor
        )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IServiceProvider ServiceProvider
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    throw new Exception(
                        "HttpContextClientScopeServiceProviderAccessor should only be used in a web request scope!");
                }

                return httpContext.RequestServices;
            }
        }
    }
}