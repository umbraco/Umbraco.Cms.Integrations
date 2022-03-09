using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public interface IShopifyService
    {
        EditorSettings GetApiConfiguration();

        string GetAuthorizationUrl();

        Task<string> GetAccessToken(OAuthRequestDto request);

        Task<ResponseDto<ProductsListDto>> ValidateAccessToken();

        void RevokeAccessToken();

        Task<ResponseDto<ProductsListDto>> GetResults();
    }
}
