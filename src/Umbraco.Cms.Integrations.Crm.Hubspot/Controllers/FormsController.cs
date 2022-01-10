using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Helpers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmHubspot")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        public async Task<List<HubspotFormDto>> GetAll()
        {
            //TODO: Errorhandling
            var httpClient = Http.GetHttpClient();

            var hubspotApiKey = ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.ApiKey"];

            var response = await httpClient.GetAsync("https://api.hubapi.com/forms/v2/forms?hapikey=" + hubspotApiKey);
            var forms = await response.Content.ReadAsStringAsync();
            var hubspotForms = HubspotForms.FromJson(forms);

            var formsDto = new List<HubspotFormDto>();
            foreach (var hubspotForm in hubspotForms)
            {
                var hubspotFormDto = new HubspotFormDto
                {
                    Name = hubspotForm.Name,
                    PortalId = hubspotForm.PortalId.ToString(),
                    Id = hubspotForm.Guid,
                    Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                };
                formsDto.Add(hubspotFormDto);
            }

            return formsDto;
        }
    }
}
