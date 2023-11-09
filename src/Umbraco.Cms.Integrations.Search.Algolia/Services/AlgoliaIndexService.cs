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

        public async Task<Result> PushData(string name, List<Record> payload = null)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                await index.SaveObjectsAsync(payload != null
                    ? payload
                    : new List<Record> {
                        new Record {
                            ObjectID = Guid.NewGuid().ToString(),
                            Data = new Dictionary<string, object>()}
                    }, autoGenerateObjectId: false);

                if (payload == null)
                    await index.ClearObjectsAsync();

                return Result.Ok();
            }
            catch (AlgoliaException ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> UpdateData(string name, Record record)
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

                return Result.Ok();
            }
            catch (AlgoliaException ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> DeleteData(string name, string objectId)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                await index.DeleteObjectAsync(objectId);

                return Result.Ok();
            }
            catch (AlgoliaException ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> DeleteIndex(string name)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                await index.DeleteAsync();

                return Result.Ok();
            }
            catch (AlgoliaException ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<bool> IndexExists(string name)
        {
            var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

            var indices = await client.ListIndicesAsync();

            return indices.Items.Any(p => p.Name == name);
        }
    }
}
