using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public interface IShopifyAuthorizationService
    {
        string GetAuthorizationUrl();

        string GetAccessToken(string code);

        Task<string> GetAccessTokenAsync(string code);
    }
}
