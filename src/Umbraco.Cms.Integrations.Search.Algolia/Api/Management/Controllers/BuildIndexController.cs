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

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Management.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
public class BuildIndexController : SearchControllerBase
{
    private readonly IAlgoliaIndexDefinitionStorage<AlgoliaIndex> _indexStorage;
    private readonly IAlgoliaIndexService _indexService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IContentService _contentService;
    private readonly ILogger<BuildIndexController> _logger;
    private readonly IUserService _userService;
    private readonly IPublishedUrlProvider _urlProvider;
    private readonly IAlgoliaSearchPropertyIndexValueFactory _algoliaSearchPropertyIndexValueFactory;
    private readonly IRecordBuilderFactory _recordBuilderFactory;

    public BuildIndexController(
        IAlgoliaIndexDefinitionStorage<AlgoliaIndex> indexStorage,
        IUmbracoContextFactory umbracoContextFactory,
        IContentService contentService,
        ILogger<BuildIndexController> logger,
        IAlgoliaIndexService indexService,
        IUserService userService,
        IPublishedUrlProvider urlProvider,
        IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory,
        IRecordBuilderFactory recordBuilderFactory)
    {
        _indexStorage = indexStorage;
        _umbracoContextFactory = umbracoContextFactory;
        _contentService = contentService;
        _logger = logger;
        _indexService = indexService;
        _userService = userService;
        _urlProvider = urlProvider;
        _algoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;
        _recordBuilderFactory = recordBuilderFactory;
    }

    [HttpPost("index/build")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> BuildIndex([FromBody] IndexConfiguration indexConfiguration)
    {
        AlgoliaIndex index = _indexStorage.GetById(indexConfiguration.Id);
        List<Record> payload = [];

        IEnumerable<ContentTypeDto>? indexContentData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData);
        if (indexContentData is null)
        {
            // TODO => handle null result
            return BadRequest();
        }

        foreach (var contentDataItem in indexContentData)
        {
            using var ctx = _umbracoContextFactory.EnsureUmbracoContext();
            var contentType = ctx.UmbracoContext.Content?.GetContentType(contentDataItem.Alias);

            if (contentType is null)
            {
                // TODO => handle null content type
                continue;
            }

            // use GetPagedOfTypes as filter is nullable here, but not on GetPagedOfType
            var contentItems = _contentService.GetPagedOfTypes([contentType.Id], 0, int.MaxValue, out _, null);

            _logger.LogInformation("Building index for {ContentType} with {Count} items", contentDataItem.Alias, contentItems.Count());

            foreach (var contentItem in contentItems.Where(p => !p.Trashed))
            {
                var record = new ContentRecordBuilder(_userService, _urlProvider, _algoliaSearchPropertyIndexValueFactory, _recordBuilderFactory, _umbracoContextFactory)
                    .BuildFromContent(contentItem, (p) => contentDataItem.Properties.Any(q => q.Alias == p.Alias))
                    .Build();

                payload.Add(record);
            }
        }

        var result = await _indexService.PushData(index.Name, payload);

        return Ok(result);
    }
}
