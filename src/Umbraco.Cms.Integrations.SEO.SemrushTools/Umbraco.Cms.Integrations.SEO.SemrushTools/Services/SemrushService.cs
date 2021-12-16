using Newtonsoft.Json;

using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Core.Services;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Services
{
    public class SemrushService: ISemrushService<TokenDto>
    {
        private const string TOKEN_DB_KEY = "Umbraco.Cms.Integrations.Semrush.TokenDbKey";

        private readonly IKeyValueService _kvService;

        public SemrushService(IKeyValueService kvService)
        {
            _kvService = kvService;
        }

        public bool TryGetParameters(out TokenDto obj)
        {
            obj = new TokenDto();

            if (string.IsNullOrEmpty(_kvService.GetValue(TOKEN_DB_KEY))) return false;

            obj = JsonConvert.DeserializeObject<TokenDto>(_kvService.GetValue(TOKEN_DB_KEY));

            return true;
        }

        public void SaveParameters(string serializedParams)
        { 
            _kvService.SetValue(TOKEN_DB_KEY, serializedParams);
        }
    }
}
