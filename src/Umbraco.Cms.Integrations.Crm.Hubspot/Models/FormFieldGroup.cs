using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models
{
    public class FormFieldGroup
    {
        [JsonPropertyName("fields")]
        public List<Field> Fields { get; set; }
    }
}
