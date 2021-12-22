using Newtonsoft.Json;

using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Core.Services;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Services
{
    public class SemrushService: ISemrushService<TokenDto>
    {
        private readonly IKeyValueService _kvService;

        public SemrushService(IKeyValueService kvService)
        {
            _kvService = kvService;
        }

        public bool TryGetParameters(string key, out TokenDto obj)
        {
            obj = new TokenDto();

            if (string.IsNullOrEmpty(_kvService.GetValue(key))) return false;

            obj = JsonConvert.DeserializeObject<TokenDto>(_kvService.GetValue(key));

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
