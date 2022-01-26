using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos
{
    /// <summary>
    /// OAuth response object
    /// IsAccessTokenValid = access token saved into the database
    /// IsAccessTokenExpired = access token expired, use refresh token to renew
    /// </summary>
    public class OAuthResponseDto
    {
        public OAuthResponseDto()
        {
            Forms = new List<HubspotFormDto>();
        }

        [JsonProperty("isAccessTokenValid")]
        public bool IsAccessTokenValid { get; set; }

        [JsonProperty("isAccessTokenExpired")]
        public bool IsAccessTokenExpired { get; set; }

        public List<HubspotFormDto> Forms { get; set; }
    }
}
