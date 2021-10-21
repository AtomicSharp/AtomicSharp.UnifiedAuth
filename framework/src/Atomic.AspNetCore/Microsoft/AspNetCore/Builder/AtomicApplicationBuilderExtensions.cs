using Atomic.AspNetCore.Security;

namespace Microsoft.AspNetCore.Builder
{
    public static class AtomicApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAbpSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AtomicSecurityHeadersMiddleware>();
        }
    }
}