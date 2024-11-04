using System.Text.Json;
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
                bool requiresUpdate = false;

                var indexSerializedData = JsonSerializer.Deserialize<IEnumerable<ContentTypeDto>>(index.SerializedData);
                if (indexSerializedData == null) continue;

                foreach (var data in indexSerializedData.Where(x => x.ContentType != null))
                {
                    requiresUpdate = true;

                    var contentType = _contentTypeService.Get(data.ContentType!.Alias);
                    if (contentType != null)
                    {
                        data.Id = contentType.Id;
                    }

                    data.Icon = data.ContentType!.Icon;
                    data.Alias = data.ContentType!.Alias;
                    data.Name = data.ContentType!.Name;
                }

                if (requiresUpdate)
                {
                    index.SerializedData = JsonSerializer.Serialize(indexSerializedData);
                    scope.Database.Update(index);
                }
            }

            scope.Complete();
        }
    }
}
