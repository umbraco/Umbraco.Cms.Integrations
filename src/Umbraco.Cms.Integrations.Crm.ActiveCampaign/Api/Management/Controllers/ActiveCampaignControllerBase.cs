using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Configuration;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public class ActiveCampaignControllerBase : Controller
    {
        protected readonly ActiveCampaignSettings Settings;

        protected readonly IHttpClientFactory HttpClientFactory;

        protected const string ApiPath = "/api/3/forms";

        public ActiveCampaignControllerBase(IOptions<ActiveCampaignSettings> options, IHttpClientFactory httpClientFactory)
        {
            Settings = options.Value;

            HttpClientFactory = httpClientFactory;
        }
    }
}
