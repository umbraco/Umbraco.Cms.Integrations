using System;
using System.Collections.Generic;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Sorting;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Services;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search;
using System.Threading.Tasks;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.ModelBinders;
using Umbraco.Cms.Web.Common.Attributes;

#else
using System.Web.Http;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCommerceCommerceTools")]
    public class ResourceController : UmbracoAuthorizedJsonController
    {
        private readonly ICommerceToolsService _commerceToolsService;

        public ResourceController(ICommerceToolsService commerceToolsService)
        {
            _commerceToolsService = commerceToolsService;
        }

        public bool GetApi() => true;

        [HttpGet]
        [ActionName("GetProductById")]
        public async Task<Product> GetProductByIdAsync(Guid id, string languageCode = null)
        {
            return await _commerceToolsService.GetProductByIdAsync(id, languageCode);
        }

        [HttpPost]
        [ActionName("GetProductByIds")]
        public async Task<IEnumerable<Product>> GetProductByIdsAsync([FromBody] IdsRequest model)
        {
            return await _commerceToolsService.GetProductsByIdsAsync(model.Ids, model.LanguageCode);
        }

        [HttpPost]
        [ActionName("GetCategoryByIds")]
        public async Task<IEnumerable<Category>> GetCategoryByIdsAsync([FromBody] IdsRequest model)
        {
            return await _commerceToolsService.GetCategoriesByIdsAsync(model.Ids, model.LanguageCode);
        }

        [HttpGet]
        [ActionName("GetCategoryById")]
        public async Task<Category> GetCategoryByIdAsync(Guid id, string languageCode = null)
        {
            return await _commerceToolsService.GetCategoryByIdAsync(id, languageCode);
        }

        [HttpGet]
        [ActionName("GetPagedCategory")]
        public async Task<PagedResults<Category>> GetPagedCategoryAsync(
            int pageNumber,
            int pageSize,
            CategorySortingProperty orderBy = CategorySortingProperty.None,
            Direction orderDirection = Direction.Ascending,
            string languageCode = null,
            string terms = "")
        {
            return await _commerceToolsService.GetPagedCategoriesAsync(pageNumber, pageSize, orderBy, orderDirection, languageCode, terms);
        }

        [HttpGet]
        [ActionName("GetPagedProduct")]
        public async Task<PagedResults<Product>> GetPagedProductAsync(
            int pageNumber,
            int pageSize,
            ProductSortingProperty orderBy = ProductSortingProperty.None,
            Direction orderDirection = Direction.Ascending,
            string languageCode = null,
            string terms = ""
            )
        {
            return await _commerceToolsService.GetPagedProductsAsync(pageNumber, pageSize, orderBy, orderDirection, languageCode, terms);
        }
    }
}
