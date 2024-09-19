using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaNullGeolocationProvider : IAlgoliaGeolocationProvider
    {
        public async Task<List<GeolocationEntity>> GetGeolocationAsync(IContent content) => null;
    }
}
