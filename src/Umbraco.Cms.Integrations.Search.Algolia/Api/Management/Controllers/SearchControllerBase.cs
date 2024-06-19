using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiController]
[BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}/search")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi(Constants.ManagementApi.GroupName)]
public abstract class SearchControllerBase : Controller
{
    
}
