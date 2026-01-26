using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public interface ITokenService
    {
        bool TryGetParameters(string key, out string token);

        void SaveParameters(string key, string serializedParams);

        void RemoveParameters(string key);
    }
}
