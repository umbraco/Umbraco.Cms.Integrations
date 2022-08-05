using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class ResponseDto<T>
        where T : class
    {
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}
