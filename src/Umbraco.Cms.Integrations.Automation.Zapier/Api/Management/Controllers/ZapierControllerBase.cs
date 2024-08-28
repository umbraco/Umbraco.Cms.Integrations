using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public class ZapierControllerBase : Controller
    {
        private readonly ZapierSettings ZapierSettings;

        protected IUserValidationService _userValidationService;

        public ZapierControllerBase(IOptions<ZapierSettings> options, IUserValidationService userValidationService)
        {
            ZapierSettings = options.Value;

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
