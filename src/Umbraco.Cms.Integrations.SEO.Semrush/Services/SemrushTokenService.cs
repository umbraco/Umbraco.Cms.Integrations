using System.Text.Json;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Services
{
    public class SemrushTokenService: ISemrushTokenService
    {
        private readonly IKeyValueService _kvService;

        public SemrushTokenService(IKeyValueService kvService)
        {
            _kvService = kvService;
        }

        public bool TryGetParameters(string key, out TokenDto tokenDto)
        {
            tokenDto = new TokenDto();

            if (string.IsNullOrEmpty(_kvService.GetValue(key))) return false;

            tokenDto = JsonSerializer.Deserialize<TokenDto>(_kvService.GetValue(key));

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
