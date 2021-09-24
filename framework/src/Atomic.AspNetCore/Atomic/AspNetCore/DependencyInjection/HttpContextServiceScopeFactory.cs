using System;
using Atomic.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.AspNetCore.DependencyInjection
{
    [ExposeServices(
        typeof(IHybridServiceScopeFactory),
        typeof(HttpContextServiceScopeFactory)
    )]
    [Dependency(ReplaceServices = true)]
    public class HttpContextServiceScopeFactory : IHybridServiceScopeFactory, ITransientDependency
    {
        public HttpContextServiceScopeFactory(
            IHttpContextAccessor httpContextAccessor,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            HttpContextAccessor = httpContextAccessor;
            ServiceScopeFactory = serviceScopeFactory;
        }

        protected IHttpContextAccessor HttpContextAccessor { get; }

        protected IServiceScopeFactory ServiceScopeFactory { get; }

        public IServiceScope CreateScope()
        {
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return ServiceScopeFactory.CreateScope();
            }

            return new NonDisposedHttpContextServiceScope(httpContext.RequestServices);
        }

        protected class NonDisposedHttpContextServiceScope : IServiceScope
        {
            public NonDisposedHttpContextServiceScope(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; }

            public void Dispose()
            {
            }
        }
    }
}