using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmHubspot")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        internal static Func<HttpClient> ClientFactory = () => s_client;

        public async Task<List<HubspotFormDto>> GetAll()
        {
            var hubspotApiKey = ConfigurationManager.AppSettings["Umbraco.Cms.Integrations.Crm.Hubspot.ApiKey"];

            var response = await ClientFactory().GetAsync("https://api.hubapi.com/forms/v2/forms?hapikey=" + hubspotApiKey);
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
