
namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models
{
    public class OAuthCodeExchange
    {
        public string State { get; set; }

        public string CodeVerifier { get; set; }

        public string CodeChallenge { get; set; }
    }
}
