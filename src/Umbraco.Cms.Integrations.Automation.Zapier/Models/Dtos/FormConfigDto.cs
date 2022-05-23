using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos
{
    public class FormConfigDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("formName")]
        public string FormName { get; set; }

        [JsonProperty("hookUrl")]
        public string HookUrl { get; set; }
    }
}
