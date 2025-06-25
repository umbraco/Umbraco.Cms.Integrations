using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
[BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}/search")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi(Constants.ManagementApi.ApiName)]
public abstract class SearchControllerBase : ControllerBase
{
    protected IActionResult OperationStatusResult(OperationStatus status, string message) =>
        status switch
        {
            OperationStatus.EmptyIndexData => BadRequest(new ProblemDetailsBuilder()
                .WithTitle(message)
                .Build()),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetailsBuilder()
                .WithTitle(message)
                .Build())
        };
}
