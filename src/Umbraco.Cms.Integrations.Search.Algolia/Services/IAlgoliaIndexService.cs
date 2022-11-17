using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaIndexService
    {
        string PushData(string name, List<Record> payload = null);

        Task<string> UpdateData(string name, Record record); 

        Task<string> DeleteData(string name, string objectId);
    }
}
