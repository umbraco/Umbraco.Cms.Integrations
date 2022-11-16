using Algolia.Search.Clients;
using Algolia.Search.Models.Search;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Integrations.Search.Algolia.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaSearchService : IAlgoliaSearchService<SearchResponse<Record>>
    {
        private readonly AlgoliaSettings _settings;

        public AlgoliaSearchService(IOptions<AlgoliaSettings> options)
        {
            _settings = options.Value;
        }

        public SearchResponse<Record> Search(string indexName, string query)
        {
            var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);
            
            var index = client.InitIndex(indexName);

            var results = index.Search<Record>(new Query(query));

            return results;
        }
    }
}
