
namespace Umbraco.Integrations.Library.Interfaces
{
    public interface ITokenService
    {
        bool TryGetParameters<T>(string key, out T tokenDto) where T : class;

        void SaveParameters(string key, string serializedParams);

        void RemoveParameters(string key);
    }
}
