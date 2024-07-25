using Umbraco.Cms.Integrations.Commerce.Shopify.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Services
{
    public interface IShopifyService
    {
        EditorSettings GetApiConfiguration();

        Task<ResponseDto<ProductsListDto>> ValidateAccessToken();

        void RevokeAccessToken();

        Task<ResponseDto<ProductsListDto>> GetResults(string pageInfo);

        Task<ResponseDto<ProductsListDto>> GetProductsByIds(long[] ids);

        Task<int> GetCount();
    }
}
