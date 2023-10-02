using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos
{
    /// <summary>
    /// Hubspot response generic object for API/OAuth setups.
    /// IsValid = API key exists in web.config or OAuth access token is saved into the database
    /// IsExpired = API key from web.config causes the Hubspot API to return an 401 response or OAuth access token is expired => use refresh token to renew 
    /// </summary>
    public class ResponseDto: IData
    {
        public ResponseDto()
        {
            Forms = new List<HubspotFormDto>();
        }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isExpired")]
        public bool IsExpired { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("forms")]
        public List<HubspotFormDto> Forms { get; set; }
    }

    public interface IData
    {
        List<HubspotFormDto> Forms { get; set; }
    }
}
