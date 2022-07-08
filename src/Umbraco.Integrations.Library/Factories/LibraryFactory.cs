using System.Net.Http;

using Umbraco.Integrations.Library.Interfaces;
using Umbraco.Integrations.Library.Services;

namespace Umbraco.Integrations.Library.Factories
{
    public class LibraryFactory : ILibraryFactory
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient _client = new HttpClient();

        private readonly ITokenService _tokenService;

        private readonly ICacheHelper _cacheHelper;

        private readonly ILoggingService _loggingService;

        private readonly IUserValidationService _userValidationService;

        public LibraryFactory(ITokenService tokenService, ICacheHelper cacheHelper, ILoggingService loggingService, IUserValidationService userValidationService)
        {
            _tokenService = tokenService;

            _cacheHelper = cacheHelper;

            _loggingService = loggingService;

            _userValidationService = userValidationService;
        }

        public HttpClient CreateClient() => _client;

        public ITokenService CreateTokenService() => _tokenService;

        public ICacheHelper CreateCacheHelper() => _cacheHelper;

        public ILoggingService CreateLoggingService() => _loggingService;

        public ILoggingService CreateUserValidationService() => _userValidationService;
    }
}
