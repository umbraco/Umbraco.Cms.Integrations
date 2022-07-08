
using System.Net.Http;
using Umbraco.Integrations.Library.Services;

namespace Umbraco.Integrations.Library.Interfaces
{
    public interface ILibraryFactory
    {
        HttpClient CreateClient();

        ITokenService CreateTokenService();

        ICacheHelper CreateCacheHelper();

        ILoggingService CreateLoggingService();
    }
}
