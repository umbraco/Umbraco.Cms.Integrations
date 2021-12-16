
namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Services
{
    public interface ISemrushService<T>
    {
        bool TryGetParameters(out T obj);

        void SaveParameters(string serializedParams);
    }
}
