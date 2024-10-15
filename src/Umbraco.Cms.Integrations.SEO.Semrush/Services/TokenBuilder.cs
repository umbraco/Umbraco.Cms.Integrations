namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public class TokenBuilder
    {
        private const string ClientId = "umbraco";

        private Dictionary<string, string> _requestData;

        private void Initialize()
        {
            _requestData = new Dictionary<string, string>
            {
                {"client_id", ClientId}
            };
        }

        public TokenBuilder WithClientId(string clientId)
        {
            _requestData["client_id"] = clientId;
            
            return this;
        }

        public TokenBuilder WithClientSecret(string clientSecret)
        {
            _requestData.Add("client_secret", clientSecret);

            return this;
        }

        public TokenBuilder ForAccessToken(string code)
        {
            Initialize();

            _requestData.Add("code", code);
            _requestData.Add("grant_type", "authorization_code");
            _requestData.Add("redirect_uri", "/oauth2/umbraco/success");

            return this;
        }

        public TokenBuilder ForRefreshToken(string refreshToken)
        {
            Initialize();
            
            _requestData.Add("refresh_token", refreshToken);
            _requestData.Add("grant_type", "refresh_token");

            return this;
        }

        public Dictionary<string, string> Build() => _requestData;
    }
}
