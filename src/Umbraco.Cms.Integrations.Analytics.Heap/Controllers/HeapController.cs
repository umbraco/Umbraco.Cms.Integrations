using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco.Cms.Integrations.Analytics.Heap.Controllers;

public class HeapController : UmbracoApiController
{
    public IActionResult Test()
    {
        return Ok();
    }
}
