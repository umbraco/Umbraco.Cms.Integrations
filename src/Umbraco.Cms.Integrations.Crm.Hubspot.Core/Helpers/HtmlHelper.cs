using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.ViewModels;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Helpers
{
    public static class HubspotHtmlExtensions
    {
        public static IHtmlContent RenderHubspotForm(this IHtmlHelper<dynamic> htmlHelper, HubspotFormViewModel hubspotFormViewModel)
        {
            return htmlHelper.Partial("~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/Render/HubspotForm.cshtml", hubspotFormViewModel ?? new HubspotFormViewModel());
        }
    }
}
