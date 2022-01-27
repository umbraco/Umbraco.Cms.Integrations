
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public interface ISemrushTokenService
    {
        bool TryGetParameters(string key, out TokenDto tokenDto);

        void SaveParameters(string key, string serializedParams);

        void RemoveParameters(string key);
    }
}
