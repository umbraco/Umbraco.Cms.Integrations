using System.Collections.Generic;
using System.Text.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models
{
    public class HubspotForms
    {
        public static List<HubspotForm> Forms { get; set; }

        public static List<HubspotForm> FromJson(string json) => JsonSerializer.Deserialize<List<HubspotForm>>(json);
    }
}
