using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public class DynamicsControllerBase : Controller
    {
    }
}
