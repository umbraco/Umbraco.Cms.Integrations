using System.Text.Json;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;

namespace Umbraco.Cms.Integrations.Search.Algolia.Migrations
{
    public class UpdateAlgoliaIndicesPostUpgrade : MigrationBase
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IContentTypeService _contentTypeService;

        public UpdateAlgoliaIndicesPostUpgrade(IMigrationContext context, IScopeProvider scopeProvider, IContentTypeService contentTypeService)
            : base(context)
        {
            _scopeProvider = scopeProvider;
            _contentTypeService = contentTypeService;
        }

        protected override void Migrate()
        {
            using var scope = _scopeProvider.CreateScope();

            var indices = scope.Database.Fetch<AlgoliaIndex>();

            foreach (var index in indices)
            {
                var indexSerializedData = JsonSerializer.Deserialize<IEnumerable<AlgoliaMigrationIndexData>>(index.SerializedData);
                if (indexSerializedData == null) continue;

                foreach (var data in indexSerializedData.Where(x => x.ContentType != null))
                {
                    var contentTypeDto = new ContentTypeDto();

                    var contentType = _contentTypeService.Get(data.ContentType.Alias);
                    if (contentType != null)
                    {
                        contentTypeDto.Id = contentType.Id;
                    }

                    contentTypeDto.Icon = data.ContentType.Icon;
                    contentTypeDto.Alias = data.ContentType.Alias;
                    contentTypeDto.Name = data.ContentType.Name;
                    contentTypeDto.Properties = data.Properties.Select(x => new ContentTypePropertyDto
                    {
                        Alias = x.Alias,
                        Name = x.Name,
                        Icon = x.Icon,
                        Selected = true
                    });

                    index.SerializedData = JsonSerializer.Serialize(contentTypeDto);
                    scope.Database.Update(index);
                }
            }

            scope.Complete();
        }

        private class AlgoliaMigrationIndexData
        {
            [JsonPropertyName("contentType")]
            public AlgoliaMigrationIndexContentType ContentType { get; set; }

            [JsonPropertyName("properties")]
            public IEnumerable<AlgoliaMigrationIndexProperty> Properties { get; set; }
        }

        private class AlgoliaMigrationIndexContentType
        {
            [JsonPropertyName("alias")]
            public string Alias { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("icon")]
            public string Icon { get; set; }
        }

        private class AlgoliaMigrationIndexProperty
        {
            [JsonPropertyName("alias")]
            public string Alias { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("icon")]
            public string Icon { get; set; }
        }
    }
}
