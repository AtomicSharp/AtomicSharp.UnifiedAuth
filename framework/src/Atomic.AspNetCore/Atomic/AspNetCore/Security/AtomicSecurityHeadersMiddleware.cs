using System.Collections.Generic;
using System.Threading.Tasks;
using Atomic.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Atomic.AspNetCore.Security
{
    public class AtomicSecurityHeadersMiddleware : IMiddleware, ITransientDependency
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            AddHeaderIfNotExists(context, "X-Content-Type-Options", "nosniff");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            AddHeaderIfNotExists(context, "X-Frame-Options", "SAMEORIGIN");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
            const string contentSecurityPolicy =
                "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
            // once for standards compliant browsers
            AddHeaderIfNotExists(context, "Content-Security-Policy", contentSecurityPolicy);
            // and once again for IE
            AddHeaderIfNotExists(context, "X-Content-Security-Policy", contentSecurityPolicy);

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
            AddHeaderIfNotExists(context, "Referrer-Policy", "no-referrer");

            await next.Invoke(context);
        }

        protected virtual void AddHeaderIfNotExists(HttpContext context, string key, string value)
        {
            context.Response.Headers.AddIfNotContains(new KeyValuePair<string, StringValues>(key, value));
        }
    }
}