using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public interface IAlgoliaGeolocationProvider
    {
        Task<List<GeolocationEntity>> GetGeolocationAsync();
    }
}
