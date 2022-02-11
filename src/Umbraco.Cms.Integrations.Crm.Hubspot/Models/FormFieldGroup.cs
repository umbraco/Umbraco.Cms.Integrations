using System.Collections.Generic;
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class FormFieldGroup
    {
        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }
}
