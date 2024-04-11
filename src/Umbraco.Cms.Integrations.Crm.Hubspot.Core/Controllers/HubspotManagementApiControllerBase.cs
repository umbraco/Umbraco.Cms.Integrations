using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers
{
    [ApiController]
    [MapToApi(Constants.ManagementApiConfiguration.ApiName)]
    public class HubspotManagementApiControllerBase : Controller
    {
    }
}
