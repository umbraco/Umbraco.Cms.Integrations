﻿
using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos
{
    public class AuthorizationRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
