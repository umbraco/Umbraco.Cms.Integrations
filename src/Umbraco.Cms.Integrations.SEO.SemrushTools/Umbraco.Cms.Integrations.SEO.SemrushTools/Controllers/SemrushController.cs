using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.SemrushTools.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Controllers
{
    [PluginController("UmbracoCmsIntegrationsSemrush")]
    public class SemrushController: UmbracoAuthorizedJsonController
    {
        private readonly ISemrushService<TokenDto> _semrushService;

        public SemrushController(ISemrushService<TokenDto> semrushService)
        {
            _semrushService = semrushService;
        }

        [HttpGet]
        public string Ping() => "test API";

        [HttpGet]
        public TokenDto GetTokenDetails()
        {
            return _semrushService.TryGetParameters(out TokenDto tokenDto) ? tokenDto : new TokenDto();
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto request)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44358/");

            var requestData = new Dictionary<string, string>();
            requestData.Add("code", request.Code);

            var response = await client.PostAsync("/authorizationhubapi/semrush/access_token", 
                new FormUrlEncodedContent(requestData));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                _semrushService.SaveParameters(result);

                return result;
            }

            return "error";
        }

        [HttpGet]
        public async Task<RelatedPhrasesDto> GetRelatedPhrases(string phrase)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://oauth.semrush.com/");

            _semrushService.TryGetParameters(out TokenDto token);

            var response = await client.GetAsync($"api/v1/keywords/phrase_related?access_token={token.AccessToken}&phrase={phrase}&database=us");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var relatedPhrasesDto = JsonConvert.DeserializeObject<RelatedPhrasesDto>(responseContent);

                var l = new List<Dictionary<string, string>>();

                for (int i = 0; i < relatedPhrasesDto.Data.Rows.Count; i++)
                {
                    var d = new Dictionary<string, string>();

                    for (int j = 0; j < relatedPhrasesDto.Data.ColumnNames.Length; j++)
                    {
                        d.Add(relatedPhrasesDto.Data.ColumnNames[j], relatedPhrasesDto.Data.Rows[i][j]);
                    }

                    l.Add(d);
                }

                return relatedPhrasesDto;
            }

            return new RelatedPhrasesDto();
        }


    }
}
