using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Atomic.AspNetCore.RequestLocalization
{
    public static class AtomicRequestCultureCookieHelper
    {
        public static void SetCultureCookie(
            HttpContext httpContext,
            RequestCulture requestCulture
        )
        {
            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(requestCulture),
                new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                }
            );
        }
    }
}