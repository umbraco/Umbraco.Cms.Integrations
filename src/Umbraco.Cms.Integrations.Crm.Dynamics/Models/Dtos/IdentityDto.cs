using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class IdentityDto
    {
        public bool IsAuthorized { get; set; }

        [JsonProperty("systemuserid")]
        public string UserId { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        public ErrorDto Error { get; set; }
    }
}
