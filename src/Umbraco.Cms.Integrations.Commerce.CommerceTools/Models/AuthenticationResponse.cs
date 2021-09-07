using System.Runtime.Serialization;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models
{
    [DataContract]
    internal class AuthenticationResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
    }
}
