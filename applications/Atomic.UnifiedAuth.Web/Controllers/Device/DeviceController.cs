﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atomic.UnifiedAuth.Web.Controllers.Consent;
using Atomic.UnifiedAuth.Web.Security;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Atomic.UnifiedAuth.Web.Controllers.Device
{
    [Authorize]
    [SecurityHeaders]
    public class DeviceController : Controller
    {
        private readonly ConsentOptions _consentOptions;
        private readonly IEventService _events;
        private readonly IOptions<IdentityServerOptions> _identityServerOptions;
        private readonly IDeviceFlowInteractionService _interaction;
        private readonly IStringLocalizer<DeviceController> _localizer;

        public DeviceController(
            IDeviceFlowInteractionService interaction,
            IEventService eventService,
            IOptions<IdentityServerOptions> identityServerOptions,
            IStringLocalizer<DeviceController> localizer,
            IOptions<ConsentOptions> consentOptions
        )
        {
            _interaction = interaction;
            _events = eventService;
            _identityServerOptions = identityServerOptions;
            _localizer = localizer;
            _consentOptions = consentOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userCodeParamName = _identityServerOptions.Value.UserInteraction.DeviceVerificationUserCodeParameter;
            string userCode = Request.Query[userCodeParamName];

            // if the user code is not offered in the request
            // then provide a page for the user to input mannually
            if (string.IsNullOrWhiteSpace(userCode)) return View("UserCodeCapture");

            var vm = await BuildViewModelAsync(userCode);
            if (vm == null) return View("Error");

            vm.ConfirmUserCode = true;
            return View("UserCodeConfirmation", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCodeCapture(string userCode)
        {
            var vm = await BuildViewModelAsync(userCode);
            if (vm == null) return View("Error");

            return View("UserCodeConfirmation", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Callback(DeviceAuthorizationInputModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var result = await ProcessConsent(model);
            if (result.HasValidationError) return View("Error");

            return View("Success");
        }

        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
        {
            var result = new ProcessConsentResult();

            var request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            if (model.Action == "deny")
            {
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId,
                    request.ValidatedResources.RawScopeValues));
            }
            else if (model.Action == "allow")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (_consentOptions.EnableOfflineAccess == false)
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
                }
                else
                {
                    result.ValidationError = _localizer["MustChooseOneErrorMessage"];
                }
            }
            else
            {
                result.ValidationError = _localizer["InvalidSelectionErrorMessage"];
            }

            if (grantedConsent != null)
            {
                await _interaction.HandleRequestAsync(model.UserCode, grantedConsent);

                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                result.ViewModel = await BuildViewModelAsync(model.UserCode, model);
            }

            return result;
        }

        private async Task<DeviceAuthorizationViewModel> BuildViewModelAsync(
            string userCode,
            DeviceAuthorizationInputModel model = null
        )
        {
            var request = await _interaction.GetAuthorizationContextAsync(userCode);
            if (request != null) return CreateConsentViewModel(userCode, model, request);

            return null;
        }

        private DeviceAuthorizationViewModel CreateConsentViewModel(
            string userCode,
            DeviceAuthorizationInputModel model,
            DeviceFlowAuthorizationRequest request
        )
        {
            var vm = new DeviceAuthorizationViewModel
            {
                UserCode = userCode,
                ClientDescription = model?.ClientDescription,

                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),

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

            if (_consentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
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

        public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            return new()
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = apiScope.DisplayName ?? apiScope.Name,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new()
            {
                Value = IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = _localizer["OfflineAccessDisplayName"],
                Description = _localizer["OfflineAccessDescription"],
                Emphasize = true,
                Checked = check
            };
        }
    }
}