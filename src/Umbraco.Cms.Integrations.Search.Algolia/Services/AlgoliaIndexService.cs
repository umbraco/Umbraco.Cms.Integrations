using Algolia.Search.Clients;
using Algolia.Search.Exceptions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Integrations.Search.Algolia.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaIndexService : IAlgoliaIndexService
    {
        private readonly AlgoliaSettings _settings;

        private readonly ILogger<AlgoliaIndexService> _logger;

        public AlgoliaIndexService(IOptions<AlgoliaSettings> options, ILogger<AlgoliaIndexService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Result> PushData(string name, List<Record> payload = null)
        {
            try
            {
                var client = new SearchClient(_settings.ApplicationId, _settings.AdminApiKey);

                var index = client.InitIndex(name);

                if (payload == null)
                {
                    // Seeding an empty index: write a placeholder so the index exists, then empty it.
                    await index.SaveObjectsAsync(
                        new List<Record> {
                            new Record {
                                ObjectID = Guid.NewGuid().ToString(),
                                GeolocationData = null,
                                Data = new Dictionary<string, object>()}
                        }, autoGenerateObjectId: false);

                    await index.ClearObjectsAsync();

                    return Result.Ok();
                }

                var skipped = await AlgoliaBatchPush.SaveAsync(
                    payload,
                    batch => index.SaveObjectsAsync(batch, autoGenerateObjectId: false),
                    record => index.SaveObjectAsync(record, autoGenerateObjectId: false));

                if (skipped.Count == 0) return Result.Ok();

                foreach (var record in skipped)
                {
                    _logger.LogWarning(
                        "Algolia rejected '{Name}' ({Id}) for index {Index}, so it was left out of the build. This is usually a record over the account's maximum record size.",
                        record.Name,
                        record.Id,
                        name);
                }

                return Result.Partial(skipped.Select(p => $"{p.Name} ({p.Id})").ToList());
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
