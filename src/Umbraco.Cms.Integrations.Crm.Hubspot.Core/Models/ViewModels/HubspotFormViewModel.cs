namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.ViewModels
{
    public class HubspotFormViewModel
    {
        public string PortalId { get; set; }

        public string Id { get; set; }

        public string Region { get; set; }

        public string ScriptPath =>
            $"//js{(string.IsNullOrEmpty(Region) ? string.Empty : "-" + Region.ToLowerInvariant())}.hsforms.net/forms/shell.js";
    }
}
