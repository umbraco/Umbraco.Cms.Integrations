using System;
using Newtonsoft.Json;

using Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos;

#if NETCOREAPP
using Umbraco.Cms.Core.Services;
#else
using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Services
{
    public class TokenService: ITokenService
    {
        private readonly IKeyValueService _kvService;

        public TokenService(IKeyValueService kvService)
        {
            _kvService = kvService;
        }

        public bool TryGetParameters(string key, out string token)
        {
            if (string.IsNullOrEmpty(_kvService.GetValue(key)))
            {
                token = String.Empty;
                return false;
            }

            token = _kvService.GetValue(key);

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
