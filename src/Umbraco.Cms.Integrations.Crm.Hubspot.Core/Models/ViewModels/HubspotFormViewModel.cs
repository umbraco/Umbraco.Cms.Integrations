using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.ViewModels
{
    public class HubspotFormViewModel
    {
        [JsonPropertyName("portalId")]
        public string PortalId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        public string ScriptPath =>
            $"//js{(string.IsNullOrEmpty(Region) ? string.Empty : "-" + Region.ToLowerInvariant())}.hsforms.net/forms/shell.js";
    }
}
