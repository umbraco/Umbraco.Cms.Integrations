using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.SEO.SemrushTools.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.SemrushTools.Services;
using Umbraco.Web.Mvc;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools.Controllers
{
    [PluginController("UmbracoCmsIntegrationsSemrush")]
    public class SemrushController : BaseController
    {
        private readonly HttpClient _client;

        public SemrushController(ISemrushService<TokenDto> semrushService, ISemrushCachingService<RelatedPhrasesDto> cachingService)
        : base(semrushService, cachingService)
        {
            _client = new HttpClient();
        }

        [HttpGet]
        public string Ping() => "test API";

        [HttpGet]
        public string GetAuthorizationUrl() => SemrushAuthorizationEndpoint;


        [HttpGet]
        public TokenDto GetTokenDetails()
        {
            return SemrushService.TryGetParameters(TokenDbKey, out TokenDto tokenDto) ? tokenDto : new TokenDto();
        }

        [HttpPost]
        public void RevokeToken()
        {
            SemrushService.RemoveParameters(TokenDbKey);
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto request)
        {
            var requestData = new Dictionary<string, string> { { "code", request.Code } };

            var response = await _client.PostAsync($"{BaseAuthorizationHubAddress}access_token",
                new FormUrlEncodedContent(requestData));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                SemrushService.SaveParameters(TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpPost]
        public async Task<AuthorizationResponseDto> ValidateToken()
        {
            SemrushService.TryGetParameters(TokenDbKey, out TokenDto token);

            if (string.IsNullOrEmpty(token.AccessToken)) return new AuthorizationResponseDto { IsExpired = true };

            var response = await _client.GetAsync(string.Format(KeywordsEndpoint, "phrase_related", token.AccessToken, "ping", "us"));

            return new AuthorizationResponseDto
            {
                IsValid = response.StatusCode != HttpStatusCode.Unauthorized
            };
        }

        [HttpPost]
        public async Task<string> RefreshAccessToken()
        {
            SemrushService.TryGetParameters(TokenDbKey, out TokenDto token);

            var requestData = new Dictionary<string, string> { { "refresh_token", token.RefreshToken } };

            var response = await _client.PostAsync($"{BaseAuthorizationHubAddress}refresh_access_token", new FormUrlEncodedContent(requestData));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                SemrushService.SaveParameters(TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpGet]
        public async Task<RelatedPhrasesDto> GetRelatedPhrases(string phrase, int pageNumber, string dataSource, string method)
        {
            string cacheKey = $"{dataSource}-{method}-{phrase}";

            if (CachingService.TryGetCachedItem(out var relatedPhrasesDto, cacheKey))
            {
                relatedPhrasesDto.TotalPages = relatedPhrasesDto.Data.Rows.Count / 10;
                relatedPhrasesDto.Data.Rows = relatedPhrasesDto.Data.Rows.Skip((pageNumber - 1) * 10).Take(10).ToList();

                return relatedPhrasesDto;
            }

            SemrushService.TryGetParameters(TokenDbKey, out TokenDto token);

            var response = await _client.GetAsync(string.Format(KeywordsEndpoint, method, token.AccessToken, phrase, dataSource));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var relatedPhrasesDeserialized = JsonConvert.DeserializeObject<RelatedPhrasesDto>(responseContent);

                CachingService.AddCachedItem(cacheKey, responseContent);

                relatedPhrasesDeserialized.TotalPages = relatedPhrasesDeserialized.Data.Rows.Count / 10;
                relatedPhrasesDeserialized.Data.Rows = relatedPhrasesDeserialized.Data.Rows.Skip((pageNumber - 1) * 10).Take(10).ToList();

                return relatedPhrasesDeserialized;
            }

            return relatedPhrasesDto;
        }

        [HttpGet]
        public async Task<DataSourceDto> ImportDataSources()
        {
            var response = await _client.GetAsync($"{BaseAuthorizationHubAddress}import_datasources");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var dataSourceDto = new DataSourceDto
                {
                    Items = JsonConvert.DeserializeObject<Dictionary<string, string>>(result).Select(p =>
                        new DataSourceItemDto
                        {
                            Key = p.Key,
                            Value = p.Value
                        })
                };

                return dataSourceDto;
            }

            return new DataSourceDto();



        }


    }
}
