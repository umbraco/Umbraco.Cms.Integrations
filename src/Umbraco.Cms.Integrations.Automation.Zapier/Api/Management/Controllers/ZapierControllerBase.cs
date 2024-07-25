using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Cms.Integrations.Automation.Zapier.Models;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public class ZapierControllerBase : Controller
    {
        private readonly ZapierSettings Options;

        protected IUserValidationService _userValidationService;

        public ZapierControllerBase(IOptions<ZapierSettings> options, IUserValidationService userValidationService)
        {
            Options = options.Value;

            _userValidationService = userValidationService;
        }

        protected bool IsAccessValid()
        {
            string username = string.Empty;
            string password = string.Empty;
            string apiKey = string.Empty;

            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.ApiKeyHeaderKey,
                    out var apiKeyValues))
                apiKey = apiKeyValues.First();

            if (string.IsNullOrEmpty(apiKey) && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))) return false;

            var isAuthorized = _userValidationService.Validate(username, password, apiKey).GetAwaiter()
                .GetResult();
            if (!isAuthorized) return false;

            return true;
        }
    }
}
