using Umbraco.Integrations.Library.Dtos;
using Umbraco.Integrations.Library.Interfaces;

namespace Umbraco.Integrations.Library.Builders
{
    public class OAuthBuilder : IOAuthBuilder
    {
        private readonly OAuthDto _oAuthDto;

        public OAuthBuilder()
        {
            _oAuthDto = new OAuthDto();
        }

        public IOAuthBuilder SetAuthorizationUrl(string url)
        {
            _oAuthDto.ServiceAuthorizationUrl = string.Format(url, _oAuthDto.ClientId, _oAuthDto.RedirectUri, _oAuthDto.Scopes);
            return this;
        }

        public IOAuthBuilder SetMode(bool testing = true)
        {
            _oAuthDto.IsTestingMode = testing;
            return this;
        }

        public IOAuthBuilder SetClientId(string clientId)
        {
            _oAuthDto.ClientId = clientId;
            return this;
        }

        public IOAuthBuilder SetScopes(string scopes)
        {
            _oAuthDto.Scopes = scopes;
            return this;
        }

        public IOAuthBuilder SetServiceName(string serviceName)
        {
            _oAuthDto.ServiceName = serviceName;
            return this;
        }

        public OAuthDto Build() => _oAuthDto;

        public IOAuthBuilder SetRedirectUri(string redirectUri = "")
        {
            _oAuthDto.RedirectUri = string.IsNullOrEmpty(redirectUri) ? _oAuthDto.OAuthProxyBaseUrl : redirectUri;
            return this;
        }
    }
}
