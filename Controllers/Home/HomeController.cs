using System.Threading.Tasks;
using AtomicSharp.UnifiedAuth.Security;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace AtomicSharp.UnifiedAuth.Controllers.Home
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(
            IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment
        )
        {
            _interaction = interaction;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            if (!HttpContext.User.IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new UserDetailViewModel(await HttpContext.AuthenticateAsync());
            return View(model);
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                    // only show in development
                    message.ErrorDescription = null;
            }

            return View("Error", vm);
        }
    }
}
