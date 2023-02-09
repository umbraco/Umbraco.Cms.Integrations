using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

#if NETCOREAPP
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
#else
using System.Web;
using System.Web.Http;

using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
#endif

namespace Umbraco.Cms.Integrations.SEO.Semrush.Controllers
{
    [PluginController("UmbracoCmsIntegrationsSemrush")]
    public class SemrushController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        internal static Func<HttpClient> ClientFactory = () => s_client;

        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ISemrushTokenService _semrushTokenService;

        private readonly ICacheHelper _cacheHelper;

        private readonly TokenBuilder _tokenBuilder;

#if NETCOREAPP
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SemrushController(IWebHostEnvironment webHostEnvironment, ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper, TokenBuilder tokenBuilder)
        {
            _webHostEnvironment = webHostEnvironment;
#else
        public SemrushController(ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper, TokenBuilder tokenBuilder)
        {

#endif
            _semrushTokenService = semrushTokenService;

            _cacheHelper = cacheHelper;

            _tokenBuilder = tokenBuilder;
        }

        [HttpGet]
        public string Ping() => "test API";

        [HttpGet]
        public string GetAuthorizationUrl() => SemrushSettings.SemrushAuthorizationEndpoint;


        [HttpGet]
        public TokenDto GetTokenDetails()
        {
            return _semrushTokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto tokenDto) ? tokenDto : new TokenDto();
        }

        [HttpPost]
        public void RevokeToken()
        {
            _semrushTokenService.RemoveParameters(SemrushSettings.TokenDbKey);

            _cacheHelper.ClearCachedItems();
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] AuthorizationRequestDto request)
        {
            var requestData = _tokenBuilder.ForAccessToken(request.Code).Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{SemrushSettings.AuthProxyBaseAddress}{SemrushSettings.AuthProxyTokenEndpoint}"),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add(SemrushSettings.SemrushServiceHeaderKey.Key, SemrushSettings.SemrushServiceHeaderKey.Value);

            var response = await ClientFactory().SendAsync(requestMessage);

            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _semrushTokenService.SaveParameters(SemrushSettings.TokenDbKey, result);

                return result;
            }

            return "Error: " + result;
        }

        [HttpPost]
        public async Task<AuthorizationResponseDto> ValidateToken()
        {
            _semrushTokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto token);

            if (!token.IsAccessTokenAvailable) return new AuthorizationResponseDto { IsExpired = true };

            var response = await ClientFactory()
                .GetAsync(string.Format(SemrushSettings.SemrushKeywordsEndpoint, "phrase_related", token.AccessToken, "ping", "us"));

            return new AuthorizationResponseDto
            {
                IsValid = response.StatusCode != HttpStatusCode.Unauthorized,
                IsFreeAccount = response.Headers.TryGetValues(SemrushSettings.AllowLimitOffsetHeaderName,
                    out IEnumerable<string> values)
                    ? values.First().Equals("0")
                    : (bool?)null
            };
        }

        [HttpPost]
        public async Task<string> RefreshAccessToken()
        {
            _semrushTokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto token);

            var requestData = _tokenBuilder.ForRefreshToken(token.RefreshToken).Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{SemrushSettings.AuthProxyBaseAddress}{SemrushSettings.AuthProxyTokenEndpoint}"),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add(SemrushSettings.SemrushServiceHeaderKey.Key, SemrushSettings.SemrushServiceHeaderKey.Value);

            var response = await ClientFactory().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                _semrushTokenService.SaveParameters(SemrushSettings.TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpGet]
        public async Task<RelatedPhrasesDto> GetRelatedPhrases(string phrase, int pageNumber, string dataSource, string method)
        {
            string cacheKey = $"{dataSource}-{method}-{phrase}";

            if (_cacheHelper.TryGetCachedItem<RelatedPhrasesDto>(cacheKey, out var relatedPhrasesDto) && relatedPhrasesDto.Data != null)
            {
                relatedPhrasesDto.TotalPages = relatedPhrasesDto.Data.Rows.Count / SemrushSettings.DefaultPageSize;
                relatedPhrasesDto.Data.Rows = relatedPhrasesDto.Data.Rows
                    .Skip((pageNumber - 1) * SemrushSettings.DefaultPageSize)
                    .Take(SemrushSettings.DefaultPageSize)
                    .ToList();

                return relatedPhrasesDto;
            }

            _semrushTokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto token);

            var response = await ClientFactory()
                .GetAsync(string.Format(SemrushSettings.SemrushKeywordsEndpoint, method, token.AccessToken, phrase, dataSource));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var relatedPhrasesDeserialized = JsonConvert.DeserializeObject<RelatedPhrasesDto>(responseContent);

                if (!relatedPhrasesDeserialized.IsSuccessful) return relatedPhrasesDeserialized;

                _cacheHelper.AddCachedItem(cacheKey, responseContent);

                relatedPhrasesDeserialized.TotalPages = relatedPhrasesDeserialized.Data.Rows.Count / SemrushSettings.DefaultPageSize;
                relatedPhrasesDeserialized.Data.Rows = relatedPhrasesDeserialized.Data.Rows
                    .Skip((pageNumber - 1) * SemrushSettings.DefaultPageSize)
                    .Take(SemrushSettings.DefaultPageSize)
                    .ToList();

                return relatedPhrasesDeserialized;
            }

            return relatedPhrasesDto;
        }

        [HttpGet]
        public DataSourceDto GetDataSources()
        {
#if NETCOREAPP
            string semrushDataSourcesPath = $"{_webHostEnvironment.ContentRootPath}/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushDataSources.json";
#else
            string semrushDataSourcesPath = HttpContext.Current.Server.MapPath(
            "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushDataSources.json");
#endif

            _lock.EnterReadLock();

            try
            {
                if (!System.IO.File.Exists(semrushDataSourcesPath))
                {
                    var fs = System.IO.File.Create(semrushDataSourcesPath);
                    fs.Close();

                    return new DataSourceDto();
                }

                var content = System.IO.File.ReadAllText(semrushDataSourcesPath);
                var dataSourceDto = new DataSourceDto
                {
                    Items = JsonConvert.DeserializeObject<List<DataSourceItemDto>>(content).Select(p =>
                        new DataSourceItemDto
                        {
                            Code = p.Code,
                            Region = p.Region,
                            ResearchTypes = p.ResearchTypes,
                            GoogleSearchDomain = p.GoogleSearchDomain
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

        [HttpGet]
        public IEnumerable<ColumnDto> GetColumns()
        {
#if NETCOREAPP
            string semrushColumnsPath = $"{_webHostEnvironment.ContentRootPath}/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushColumns.json";
#else
            string semrushColumnsPath = HttpContext.Current.Server.MapPath(
            "/App_Plugins/UmbracoCms.Integrations/SEO/Semrush/semrushColumns.json");
#endif

            _lock.EnterReadLock();

            try
            {
                if (!System.IO.File.Exists(semrushColumnsPath))
                {
                    var fs = System.IO.File.Create(semrushColumnsPath);
                    fs.Close();

                    return Enumerable.Empty<ColumnDto>();
                }

                var content = System.IO.File.ReadAllText(semrushColumnsPath);
                return JsonConvert.DeserializeObject<IEnumerable<ColumnDto>>(content).Select(p =>
                    new ColumnDto
                    {
                        Name = p.Name,
                        Value = p.Value,
                        Description = p.Description
                    });

            }
            catch (FileNotFoundException ex)
            {
                return Enumerable.Empty<ColumnDto>();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }


    }
}
