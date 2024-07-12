using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services
{
    public interface IShopifyAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);
        string RefreshAccessToken();

        Task<string> RefreshAccessTokenAsync();
    }
}
