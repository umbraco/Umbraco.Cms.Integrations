using Algolia.Search.Models.Search;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;

using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetContentTypesController : SearchControllerBase
    {
        private readonly IContentTypeService _contentTypeService;

        public GetContentTypesController(
                   IAlgoliaIndexService indexService,
                   IAlgoliaSearchService<SearchResponse<Record>> searchService,
                   IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
                   IUserService userService, IPublishedUrlProvider urlProvider,
                   IContentService contentService,
                   IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory,
                   IUmbracoContextFactory umbracoContextFactory,
                   ILogger<GetContentTypesController> logger,
                   IRecordBuilderFactory recordBuilderFactory,
                   IContentTypeService contentTypeService)
                   : base(indexService,
                         searchService,
                         indexStorage,
                         userService,
                         urlProvider,
                         contentService,
                         algoliaSearchPropertyIndexValueFactory,
                         umbracoContextFactory,
                         logger,
                         recordBuilderFactory)
        {
            _contentTypeService = contentTypeService;
        }

        [HttpGet("content-type")]
        [ProducesResponseType(typeof(List<ContentTypeDto>), StatusCodes.Status200OK)]
        public IActionResult GetContentTypes() => Ok(GetContentTypes());

        [HttpGet("content-type/index/{id:int}")]
        [ProducesResponseType(typeof(List<ContentTypeDto>), StatusCodes.Status200OK)]
        public IActionResult GetContentTypesByIndexId(int id) => Ok(GetContentTypes(id));

        private List<ContentTypeDto> GetContentTypes(int? id)
        {
            IndexConfiguration indexConfiguration = new IndexConfiguration();
            var list = new List<ContentTypeDto>();

            if (id is not null)
            {
                var index = IndexStorage.GetById(id.Value);

                if (index is not null)
                {
                    indexConfiguration.Id = index.Id;
                    indexConfiguration.Name = index.Name;
                    indexConfiguration.ContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData);
                }
            }

            var contentTypes = _contentTypeService.GetAll();
            foreach (var contentType in contentTypes)
            {
                var properties = new List<ContentTypePropertyDto>();
                foreach (var propertyGroup in contentType.PropertyGroups)
                {
                    foreach (var property in propertyGroup.PropertyTypes)
                    {
                        var contentTypePropertyDto = new ContentTypePropertyDto
                        {
                            Id = property.Id,
                            Alias = property.Alias,
                            Icon = "icon-document",
                            Name = property.Name,
                            Group = propertyGroup.Name
                        };
                        if (indexConfiguration != null && indexConfiguration.ContentData != null)
                        {
                            contentTypePropertyDto.Selected = indexConfiguration.ContentData
                                .Any(p => p.Alias == contentType.Alias && p.Properties.Any(q => q.Alias == property.Alias));
                        }

                        properties.Add(contentTypePropertyDto);
                    }
                }

                list.Add(new ContentTypeDto
                {
                    Id = contentType.Id,
                    Icon = contentType.Icon,
                    Alias = contentType.Alias,
                    Name = contentType.Name,
                    Selected = indexConfiguration != null
                                    && indexConfiguration.ContentData != null
                                    && indexConfiguration.ContentData.Any(p => p.Alias == contentType.Alias),
                    Properties = properties.AsEnumerable()
                });
            }

            return list;
        }
    }
}
