using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class IdentityDto
    {
        [JsonProperty("systemuserid")]
        public string UserId { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }
    }
}
