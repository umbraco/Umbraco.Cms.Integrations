using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
#if NETCOREAPP
using System.Text.Json.Serialization;
#endif


namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models
{
    [DataContract]
    internal class AuthenticationResponse
    {
#if NETCOREAPP
        [JsonPropertyName("access_token")]
#endif
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

#if NETCOREAPP
        [JsonPropertyName("expires_in")]
#endif
        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

#if NETCOREAPP
        [JsonPropertyName("token_type")]
#endif
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
    }
}
