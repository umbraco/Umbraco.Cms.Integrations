
namespace Umbraco.Cms.Integrations.Shared.Configuration
{
    public interface IAppSettings
    {
        string this[string key] { get; }
    }
}
