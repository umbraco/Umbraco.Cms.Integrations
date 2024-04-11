using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "Integrations")]
    [Route($"{Constants.ManagementApiConfiguration.RootPath}/v1/hubspot-forms/")]
    public class HubSpotFormsControllerBase : HubspotManagementApiControllerBase
    {
        protected const string HubspotFormsApiEndpoint = "https://api.hubapi.com/forms/v2/forms";

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
