using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}/forms")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public abstract class HubspotFormsControllerBase : Controller
    {
        protected const string HubspotFormsApiEndpoint = "https://api.hubapi.com/forms/v2/forms";

        protected HubspotSettings Settings { get; }

        protected HubspotFormsControllerBase(IOptions<HubspotSettings> settingsOptions)
        {
            Settings = settingsOptions.Value;
        }

        protected HttpRequestMessage CreateRequest(string accessToken)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }

        protected IEnumerable<HubspotFormDto> ParseForms(string json, string region)
        {
            var hubspotForms = HubspotForms.FromJson(json);
            foreach (var hubspotForm in hubspotForms)
            {
                var hubspotFormDto = new HubspotFormDto
                {
                    Name = hubspotForm.Name,
                    PortalId = hubspotForm.PortalId.ToString(),
                    Id = hubspotForm.Guid,
                    Region = region,
                    Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                };

                yield return hubspotFormDto;
            }
        }
    }
}
