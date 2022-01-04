using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;
using Umbraco.Web.Mvc;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Controllers
{
    [PluginController("UmbracoCmsIntegrationsSemrush")]
    public class SemrushController : BaseController
    {
        private static HttpClient _client = new HttpClient();

        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public SemrushController(ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper)
        : base(semrushTokenService, cacheHelper)
        {
        }

        [HttpGet]
        public string Ping() => "test API";

        [HttpGet]
        public string GetAuthorizationUrl() => SemrushAuthorizationEndpoint;


        [HttpGet]
        public TokenDto GetTokenDetails()
        {
            return SemrushTokenService.TryGetParameters(TokenDbKey, out TokenDto tokenDto) ? tokenDto : new TokenDto();
        }

        [HttpPost]
        public void RevokeToken()
        {
            SemrushTokenService.RemoveParameters(TokenDbKey);
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto request)
        {
            var requestData = new Dictionary<string, string>
            {
                { "code", request.Code }
            };

            _client.DefaultRequestHeaders.Add("service_type", "SemrushAuthorization");

            var response = await _client.PostAsync($"{AuthProxyBaseAddress}{AuthProxyTokenEndpoint}",
                new FormUrlEncodedContent(requestData));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                SemrushTokenService.SaveParameters(TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpPost]
        public async Task<AuthorizationResponseDto> ValidateToken()
        {
            SemrushTokenService.TryGetParameters(TokenDbKey, out TokenDto token);

            if (!token.IsAccessTokenAvailable) return new AuthorizationResponseDto { IsExpired = true };

            var response = await _client
                .GetAsync(string.Format(SemrushKeywordsEndpoint, "phrase_related", token.AccessToken, "ping", "us"));

            return new AuthorizationResponseDto
            {
                IsValid = response.StatusCode != HttpStatusCode.Unauthorized
            };
        }

        [HttpPost]
        public async Task<string> RefreshAccessToken()
        {
            SemrushTokenService.TryGetParameters(TokenDbKey, out TokenDto token);

            var requestData = new Dictionary<string, string> { { "refresh_token", token.RefreshToken } };

            _client.DefaultRequestHeaders.Add("service_type", "SemrushReauthorization");

            var response = await _client.PostAsync($"{AuthProxyBaseAddress}{AuthProxyTokenEndpoint}", new FormUrlEncodedContent(requestData));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                SemrushTokenService.SaveParameters(TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpGet]
        public async Task<RelatedPhrasesDto> GetRelatedPhrases(string phrase, int pageNumber, string dataSource, string method)
        {
            string cacheKey = $"{dataSource}-{method}-{phrase}";

            if (CacheHelper.TryGetCachedItem<RelatedPhrasesDto>(cacheKey, out var relatedPhrasesDto) && relatedPhrasesDto.Data != null)
            {
                relatedPhrasesDto.TotalPages = relatedPhrasesDto.Data.Rows.Count / PageSize;
                relatedPhrasesDto.Data.Rows = relatedPhrasesDto.Data.Rows.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

                return relatedPhrasesDto;
            }

            SemrushTokenService.TryGetParameters(TokenDbKey, out TokenDto token);

            var response = await _client
                .GetAsync(string.Format(SemrushKeywordsEndpoint, method, token.AccessToken, phrase, dataSource));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var relatedPhrasesDeserialized = JsonConvert.DeserializeObject<RelatedPhrasesDto>(responseContent);

                if (!relatedPhrasesDeserialized.IsSuccessful) return relatedPhrasesDeserialized;

                CacheHelper.AddCachedItem(cacheKey, responseContent);

                relatedPhrasesDeserialized.TotalPages = relatedPhrasesDeserialized.Data.Rows.Count / PageSize;
                relatedPhrasesDeserialized.Data.Rows = relatedPhrasesDeserialized.Data.Rows.Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize).ToList();

                return relatedPhrasesDeserialized;
            }

            return relatedPhrasesDto;
        }

        [HttpGet]
        public DataSourceDto GetDataSources()
        {
            _lock.EnterReadLock();

            try
            {
                if (!File.Exists(SemrushDataSourcesPath))
                {
                    var fs = File.Create(SemrushDataSourcesPath);
                    fs.Close();

                    return new DataSourceDto();
                }

                var content = File.ReadAllText(SemrushDataSourcesPath);
                var dataSourceDto = new DataSourceDto
                {
                    Items = JsonConvert.DeserializeObject<Dictionary<string, string>>(content).Select(p =>
                        new DataSourceItemDto
                        {
                            Key = p.Key,
                            Value = p.Value
                        })
                };

                return dataSourceDto;
            }
            catch (FileNotFoundException ex)
            {
                return new DataSourceDto();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }


    }
}
