using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Umbraco.Cms.Core.Services;

using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiVersion("1.0")]
public class GetContentTypesController : SearchControllerBase
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;

    public GetContentTypesController(
        IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,                   
        IContentTypeService contentTypeService)                   
    {
        _indexStorage = indexStorage;
        _contentTypeService = contentTypeService;
    }

    [HttpGet("content-type", Name = Constants.OperationIds.GetContentTypes)]
    [ProducesResponseType(typeof(List<ContentTypeDto>), StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(GetContentTypes());
    
    [HttpGet("content-type/index/{id:int}")]
    [ProducesResponseType(typeof(List<ContentTypeDto>), StatusCodes.Status200OK)]
    public IActionResult GetByIndexId(int id) => Ok(GetContentTypes(id));

    private List<ContentTypeDto> GetContentTypes(int? id = null)
    {
        IndexConfiguration indexConfiguration = new();
        List<ContentTypeDto> list = [];

        if (id is not null)
        {
            var index = _indexStorage.GetById(id.Value);

            if (index is not null)
            {
                indexConfiguration.Id = index.Id;
                indexConfiguration.Name = index.Name;
                indexConfiguration.ContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData) ?? [];
            }
        }

        var contentTypes = _contentTypeService.GetAll();
        foreach (var contentType in contentTypes)
        {
            var properties = new List<ContentTypePropertyDto>();
            foreach (var propertyGroup in contentType.PropertyGroups)
            {
                if (propertyGroup.PropertyTypes is null)
                {
                    continue;
                }

                foreach (var property in propertyGroup.PropertyTypes)
                {
                    var contentTypePropertyDto = new ContentTypePropertyDto
                    {
                        Id = property.Id,
                        Alias = property.Alias,
                        Icon = "icon-document",
                        Name = property.Name,
                        Group = propertyGroup.Name ?? string.Empty,
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
                Icon = contentType.Icon ?? string.Empty,
                Alias = contentType.Alias,
                Name = contentType.Name ?? string.Empty,
                Selected = indexConfiguration != null
                                && indexConfiguration.ContentData != null
                                && indexConfiguration.ContentData.Any(p => p.Alias == contentType.Alias),
                Properties = properties.AsEnumerable()
            });
        }

        return list;
    }
}
