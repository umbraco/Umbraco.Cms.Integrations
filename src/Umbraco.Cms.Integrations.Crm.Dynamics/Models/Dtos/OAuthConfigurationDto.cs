
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class OAuthConfigurationDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }
    }
}
