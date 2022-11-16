using Algolia.Search.Clients;
using Algolia.Search.Exceptions;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Integrations.Search.Algolia.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaIndexService : IAlgoliaIndexService
    {
        private readonly AlgoliaSettings _settings;

        public AlgoliaIndexService(IOptions<AlgoliaSettings> options)
        {
            _settings = options.Value;
        }

        public string PushData(string name, List<Record> payload)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                index.SaveObjects(payload,  autoGenerateObjectId: false).Wait();

                return string.Empty;
            }
            catch(AlgoliaException ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> UpdateData(string name, Record record)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                await index.PartialUpdateObjectAsync(record);

                return string.Empty;
            }
            catch (AlgoliaException ex)
            {
                return ex.Message;
            }
        }
    }
}
