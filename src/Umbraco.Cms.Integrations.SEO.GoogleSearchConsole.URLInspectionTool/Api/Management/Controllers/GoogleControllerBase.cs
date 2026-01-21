using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Configuration;
using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using static Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.GoogleComposer;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Api.Management.Controllers;

[ApiController]
[BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi(Constants.ManagementApi.ApiName)]
public class GoogleControllerBase : ControllerBase
{
    protected IHttpClientFactory ClientFactory;

    protected readonly IGoogleAuthorizationService AuthorizationService;

    protected readonly ITokenService TokenService;

    protected readonly GoogleSearchConsoleSettings Settings;

    public GoogleControllerBase(
        IOptions<GoogleSearchConsoleSettings> options,
        ITokenService tokenService,
        IHttpClientFactory clientFactory,
        AuthorizationImplementationFactory authorizationImplementationFactory)
    {
        Settings = options.Value;
        TokenService = tokenService;
        ClientFactory = clientFactory;
        AuthorizationService = authorizationImplementationFactory(Settings.UseUmbracoAuthorization);
    }

}
