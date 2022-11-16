using System.Dynamic;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaIndexService
    {
        string PushData(string name, List<Record> payload);

        Task<string> UpdateData(string name, Record record); 
    }
}
