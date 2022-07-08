using System;

using Newtonsoft.Json;

using Umbraco.Integrations.Library.Interfaces;

#if NETCOREAPP
using Umbraco.Cms.Core.Services;
#else

using Umbraco.Core.Services;
#endif

namespace Umbraco.Integrations.Library.Services
{
    public class TokenService : ITokenService
    {
        private readonly IKeyValueService _kvService;

        public TokenService(IKeyValueService kvService)
        {
            _kvService = kvService;
        }

        public bool TryGetParameters<T>(string key, out T tokenDto) where T : class
        {
            tokenDto = (T) Activator.CreateInstance(typeof(T));

            if (string.IsNullOrEmpty(_kvService.GetValue(key))) return false;

            tokenDto = JsonConvert.DeserializeObject<T>(_kvService.GetValue(key));

            return true;
        }

        public void SaveParameters(string key, string serializedParams)
        {
            _kvService.SetValue(key, serializedParams);
        }

        public void RemoveParameters(string key)
        {
            _kvService.SetValue(key, string.Empty);
        }
    }
}
