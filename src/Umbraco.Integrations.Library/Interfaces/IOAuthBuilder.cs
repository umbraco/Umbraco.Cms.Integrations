
using Umbraco.Integrations.Library.Dtos;

namespace Umbraco.Integrations.Library.Interfaces
{
    public interface IOAuthBuilder
    {
        IOAuthBuilder SetMode(bool testing = true);

        IOAuthBuilder SetClientId(string clientId);

        IOAuthBuilder SetServiceName(string serviceName);

        IOAuthBuilder SetScopes(string scopes);

        IOAuthBuilder SetRedirectUri(string redirectUri);

        IOAuthBuilder SetAuthorizationUrl(string url);

        OAuthDto Build();
    }
}
