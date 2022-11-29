using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaIndexService
    {
        Task<Result> PushData(string name, List<Record> payload = null);

        Task<Result> UpdateData(string name, Record record); 

        Task<Result> DeleteData(string name, string objectId);

        Task<Result> DeleteIndex(string name);
    }
}
