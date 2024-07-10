using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models;
using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.Dtos;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Services
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
