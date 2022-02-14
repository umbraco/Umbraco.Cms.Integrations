using Newtonsoft.Json;
using System.Collections.Generic;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class HubspotForms
    {
        public static List<HubspotForm> Forms { get; set; }

        public static List<HubspotForm> FromJson(string json) => JsonConvert.DeserializeObject<List<HubspotForm>>(json, Constants.SerializationSettings);
    }
}
