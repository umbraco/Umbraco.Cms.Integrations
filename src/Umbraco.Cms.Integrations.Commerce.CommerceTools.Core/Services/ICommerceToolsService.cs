using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Filters;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Sorting;

#if NETCOREAPP
using Umbraco.Cms.Core;

#else
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Services
{
    public interface ICommerceToolsService
    {
        Task<PagedResults<Category>> GetCategoriesAsync(int? pageIndex = null, int? pageSize = null, string languageCode = null, CategorySorting sorting = null, params BaseFilter[] filters);
        
        Task<IEnumerable<Category>> GetCategoriesByIdsAsync(Guid[] ids, string languageCode = null);
        
        Task<Category> GetCategoryByIdAsync(Guid id, string languageCode = null);
        
        Task<PagedResults<Category>> GetPagedCategoriesAsync(int pageNumber, int pageSize, CategorySortingProperty orderBy = CategorySortingProperty.None, Direction orderDirection = Direction.Ascending, string languageCode = null, string terms = "");
        
        Task<PagedResults<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, ProductSortingProperty orderBy = ProductSortingProperty.None, Direction orderDirection = Direction.Ascending, string languageCode = null, string terms = "");
        
        Task<Product> GetProductByIdAsync(Guid id, string languageCode = null);
        
        Task<PagedResults<Product>> GetProductsAsync(int? pageIndex = null, int? pageSize = null, string languageCode = null, ProductSorting sorting = null, params BaseFilter[] filters);
        
        Task<IEnumerable<Product>> GetProductsByIdsAsync(Guid[] ids, string languageCode = null);
    }
}