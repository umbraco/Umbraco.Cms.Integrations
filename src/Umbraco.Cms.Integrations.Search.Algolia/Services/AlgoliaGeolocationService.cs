using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaGeolocationService
    {
        private readonly IAlgoliaGeolocationProvider _geolocationProvider;

        public AlgoliaGeolocationService(IServiceProvider serviceProvider)
        {
            _geolocationProvider = serviceProvider.GetService<IAlgoliaGeolocationProvider>();
        }

        public async Task<List<GeolocationEntity>> GetGeolocationDataAsync() =>
            _geolocationProvider != null
                ? await _geolocationProvider.GetGeolocationAsync()
                : new List<GeolocationEntity>();
    }
}
