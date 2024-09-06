
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class OAuthConfigurationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("isAuthorized")]
        public bool IsAuthorized { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
