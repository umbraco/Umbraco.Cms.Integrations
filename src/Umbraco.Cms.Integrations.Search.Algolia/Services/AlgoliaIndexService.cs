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

        public string PushData(string name, List<Record> payload = null)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                index.SaveObjects(payload != null
                    ? payload
                    : new List<Record> { 
                        new Record { 
                            ObjectID = Guid.NewGuid().ToString(), 
                            Data = new Dictionary<string, string>()} 
                    },  autoGenerateObjectId: false).Wait();

                if(payload == null)
                    index.ClearObjects().Wait();

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

                var obj = index.GetObjects<Record>(new[] { record.ObjectID }).FirstOrDefault();
                if (obj != null)
                    await index.PartialUpdateObjectAsync(record);
                else
                    await index.SaveObjectAsync(record, autoGenerateObjectId: false);
                
                return string.Empty;
            }
            catch (AlgoliaException ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteData(string name, string objectId)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                await index.DeleteObjectAsync(objectId);

                return string.Empty;
            }
            catch (AlgoliaException ex)
            {
                return ex.Message;
            }
        }
    }
}
