
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;

namespace Umbraco.Cms.Integrations.Search.Algolia.Controllers.ManagementApi
{
    [MapToApi(Constants.ManagementApi.ApiName)]
    public abstract class AlgoliaManagementApiControllerBase : Controller
    {
    }
}
