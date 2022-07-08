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
using Umbraco.Integrations.Library.Interfaces;
using Umbraco.Integrations.Dtos;
using Umbraco.Integrations.Library.Dtos;
using Umbraco.Integrations.Library.Services;
using Umbraco.Integrations.Library.Builders;

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
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ILibraryFactory _libraryFactory;

        private readonly TokenBuilder _tokenBuilder;

        public OAuthDto OAuthDto { get; set; }

#if NETCOREAPP
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SemrushController(IWebHostEnvironment webHostEnvironment, ILibraryFactory libraryFactory, TokenBuilder tokenBuilder)
        {
            _webHostEnvironment = webHostEnvironment;
#else
        public SemrushController(ILibraryFactory libraryFactory, TokenBuilder tokenBuilder)
        {

#endif
            _libraryFactory = libraryFactory;

            _tokenBuilder = tokenBuilder;

            OAuthDto = new OAuthBuilder()
                .SetClientId("umbraco")
                .SetScopes("user.id,domains.info,url.info,positiontracking.info")
                .SetRedirectUri("%2Foauth2%2Fumbraco%2Fsuccess")
                .SetAuthorizationUrl("https://oauth.semrush.com/oauth2/authorize?ref=0053752252&client_id={0}&redirect_uri={1}&response_type=code&scope={2}")
                .SetServiceName("Semrush")
                .SetMode(false)
                .Build();
        }

        [HttpGet]
        public string Ping() => "test API";

        [HttpGet]
        public string GetAuthorizationUrl() => OAuthDto.ServiceAuthorizationUrl;


        [HttpGet]
        public TokenDto GetTokenDetails()
        {
            var tokenService = _libraryFactory.CreateTokenService();

            return tokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto tokenDto) ? tokenDto : new TokenDto();
        }

        [HttpPost]
        public void RevokeToken()
        {
            var tokenService = _libraryFactory.CreateTokenService();

            var cacheHelper = _libraryFactory.CreateCacheHelper();

            tokenService.RemoveParameters(SemrushSettings.TokenDbKey);

            cacheHelper.ClearCachedItems();
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto request)
        {
            var client = _libraryFactory.CreateClient();

            var tokenService = _libraryFactory.CreateTokenService();

            var requestData = _tokenBuilder.ForAccessToken(request.Code).Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(String.Format(OAuthDto.OAuthProxyTokenEndpoint, OAuthDto.OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", OAuthDto.ServiceName);

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                tokenService.SaveParameters(SemrushSettings.TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpPost]
        public async Task<AuthorizationResponseDto> ValidateToken()
        {
            var client = _libraryFactory.CreateClient();

            var tokenService = _libraryFactory.CreateTokenService();

            tokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto token);

            if (!token.IsAccessTokenAvailable) return new AuthorizationResponseDto { IsExpired = true };

            var response = await client
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
            var client = _libraryFactory.CreateClient();

            var tokenService = _libraryFactory.CreateTokenService();

            tokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto token);

            var requestData = _tokenBuilder.ForRefreshToken(token.RefreshToken).Build();

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(String.Format(OAuthDto.OAuthProxyTokenEndpoint, OAuthDto.OAuthProxyBaseUrl)),
                Content = new FormUrlEncodedContent(requestData),
            };
            requestMessage.Headers.Add("service_name", OAuthDto.ServiceName);

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                tokenService.SaveParameters(SemrushSettings.TokenDbKey, result);

                return result;
            }

            return "error";
        }

        [HttpGet]
        public async Task<RelatedPhrasesDto> GetRelatedPhrases(string phrase, int pageNumber, string dataSource, string method)
        {
            var cacheHelper = _libraryFactory.CreateCacheHelper();

            var tokenService = _libraryFactory.CreateTokenService();

            string cacheKey = $"{dataSource}-{method}-{phrase}";

            if (cacheHelper.TryGetCachedItem<RelatedPhrasesDto>(cacheKey, out var relatedPhrasesDto) && relatedPhrasesDto.Data != null)
            {
                relatedPhrasesDto.TotalPages = relatedPhrasesDto.Data.Rows.Count / SemrushSettings.DefaultPageSize;
                relatedPhrasesDto.Data.Rows = relatedPhrasesDto.Data.Rows
                    .Skip((pageNumber - 1) * SemrushSettings.DefaultPageSize)
                    .Take(SemrushSettings.DefaultPageSize)
                    .ToList();

                return relatedPhrasesDto;
            }

            tokenService.TryGetParameters(SemrushSettings.TokenDbKey, out TokenDto token);

            var client = _libraryFactory.CreateClient();

            var response = await client
                .GetAsync(string.Format(SemrushSettings.SemrushKeywordsEndpoint, method, token.AccessToken, phrase, dataSource));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var relatedPhrasesDeserialized = JsonConvert.DeserializeObject<RelatedPhrasesDto>(responseContent);

                if (!relatedPhrasesDeserialized.IsSuccessful) return relatedPhrasesDeserialized;

                cacheHelper.AddCachedItem(cacheKey, responseContent);

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
