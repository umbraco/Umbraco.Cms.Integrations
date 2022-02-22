#if NETCOREAPP
using Umbraco.Cms.Core.Services;
#else
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Core.Services;
#endif


namespace Umbraco.Cms.Integrations.Crm.Hubspot.Services
{
    public class TokenService: ITokenService
    {
        private readonly IKeyValueService _kvService;

        public TokenService(IKeyValueService kvService)
        {
            _kvService = kvService;
        }

        public bool TryGetParameters(string key, out string value)
        {
            value = string.Empty;

            var serviceValue = _kvService.GetValue(key);

            if (string.IsNullOrEmpty(serviceValue)) return false;

            value = serviceValue;
            
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
