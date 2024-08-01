
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class IdentityDto
    {
        public bool IsAuthorized { get; set; }

        [JsonPropertyName("systemuserid")]
        public string UserId { get; set; }

        [JsonPropertyName("fullname")]
        public string FullName { get; set; }

        public ErrorDto Error { get; set; }
    }
}
