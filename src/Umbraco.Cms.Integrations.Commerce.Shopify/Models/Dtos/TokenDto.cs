namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class TokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("isAccessTokenAvailable")]
        public bool IsAccessTokenAvailable => !string.IsNullOrEmpty(AccessToken);
    }
}
