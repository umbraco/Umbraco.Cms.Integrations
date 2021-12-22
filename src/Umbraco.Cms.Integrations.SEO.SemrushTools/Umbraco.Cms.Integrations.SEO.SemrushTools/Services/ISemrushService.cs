
namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Services
{
    public interface ISemrushService<T>
    {
        bool TryGetParameters(string key, out T obj);

        void SaveParameters(string key, string serializedParams);

        void RemoveParameters(string key);
    }
}
