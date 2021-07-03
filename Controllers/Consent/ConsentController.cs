using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AtomicSharp.UnifiedAuth.Controllers.Consent
{
    [SecurityHeaders]
    [Authorize]
    public class ConsentController : Controller
    {
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<ConsentController> _logger;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IEventService events,
            ILogger<ConsentController> logger
        )
        {
            _interaction = interaction;
            _events = events;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await BuildViewModelAsync(returnUrl);
            return vm != null ? View("Index", vm) : View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await ProcessConsent(model);

            if (result.IsRedirect) return Redirect(result.RedirectUri);

            if (result.HasValidationError) ModelState.AddModelError(string.Empty, result.ValidationError);

            if (result.ShowView) return View("Index", result.ViewModel);

            return View("Error");
        }

        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            // ensure that return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            switch (model.Action)
            {
                case "deny":
                    grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

                    await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId,
                        request.ValidatedResources.RawScopeValues));
                    break;
                case "allow" when model.ScopesConsented != null && model.ScopesConsented.Any():
                    {
                        var scopes = model.ScopesConsented;
                        if (ConsentOptions.EnableOfflineAccess == false)
                            scopes = scopes.Where(x =>
                                x != IdentityServerConstants.StandardScopes.OfflineAccess);

                        grantedConsent = new ConsentResponse
                        {
                            RememberConsent = model.RememberConsent,
                            ScopesValuesConsented = scopes.ToArray(),
                            Description = model.ClientDescription
                        };

                        await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId,
                            request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented,
                            grantedConsent.RememberConsent));
                        break;
                    }
                case "allow":
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                    break;
                default:
                    result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
                    break;
            }

            if (grantedConsent != null)
            {
                await _interaction.GrantConsentAsync(request, grantedConsent);
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
                return CreateConsentViewModel(model, returnUrl, request);
            _logger.LogError("No consent request matching request: {returnUrl}", returnUrl);

            return null;
        }

        private static ConsentViewModel CreateConsentViewModel(
            ConsentInputModel model,
            string returnUrl,
            AuthorizationRequest request
        )
        {
            var vm = new ConsentViewModel
            {
                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                ClientDescription = model?.ClientDescription,

                ReturnUrl = returnUrl,

                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x =>
                CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeViewModel>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeViewModel(parsedScope, apiScope,
                        vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }

            if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
                apiScopes.Add(GetOfflineAccessScope(
                    vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) ||
                    model == null));

            vm.ApiScopes = apiScopes;

            return vm;
        }

        private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new()
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        private static ScopeViewModel CreateScopeViewModel(
            ParsedScopeValue parsedScopeValue,
            ApiScope apiScope,
            bool check
        )
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
                displayName += ":" + parsedScopeValue.ParsedParameter;

            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private static ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new()
            {
                Value = IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
