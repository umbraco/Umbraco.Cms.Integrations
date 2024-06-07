using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiController]
[BackOfficeRoute("time/api/v{version:apiVersion}/time")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi("time")]
public class TimeDashboardControllerBase
{
}

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "time")]
public class TimeDashboardTimesController : TimeDashboardControllerBase
{
    [HttpGet("time")]
    [ProducesResponseType(typeof(string), 200)]
    public string GetTime()
        => DateTime.Now.ToString("T");

    [HttpGet("date")]
    [ProducesResponseType(typeof(string), 200)]
    public string GetDate()
        => DateTime.Now.ToString("D");
}
