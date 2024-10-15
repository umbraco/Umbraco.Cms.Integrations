using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.SemrushGroupName)]
    public class GetRelatedPhrasesController : SemrushControllerBase
    {
        public GetRelatedPhrasesController(
            IOptions<SemrushSettings> options, 
            IWebHostEnvironment webHostEnvironment, 
            ISemrushTokenService semrushTokenService, 
            ICacheHelper cacheHelper, 
            TokenBuilder tokenBuilder, 
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
        {
        }

        [HttpGet("related-phrases")]
        [ProducesResponseType(typeof(RelatedPhrasesDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRelatedPhrases(string phrase, int pageNumber, string dataSource, string method)
        {
            string cacheKey = $"{dataSource}-{method}-{phrase}";

            if (_cacheHelper.TryGetCachedItem<RelatedPhrasesDto>(cacheKey, out var relatedPhrasesDto) && relatedPhrasesDto.Data != null)
            {
                relatedPhrasesDto.TotalPages = (int)Math.Ceiling((double)relatedPhrasesDto.Data.Rows.Count / Constants.DefaultPageSize);
                relatedPhrasesDto.Data.Rows = relatedPhrasesDto.Data.Rows
                    .Skip((pageNumber - 1) * Constants.DefaultPageSize)
                    .Take(Constants.DefaultPageSize)
                    .ToList();

                return Ok(relatedPhrasesDto);
            }

            _semrushTokenService.TryGetParameters(Constants.TokenDbKey, out TokenDto token);

            var httpClient = _clientFactory.CreateClient();
            var response = await httpClient
                .GetAsync(string.Format(Constants.SemrushKeywordsEndpoint, _settings.BaseUrl, method, token.AccessToken, phrase, dataSource));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var relatedPhrasesDeserialized = JsonSerializer.Deserialize<RelatedPhrasesDto>(responseContent);

                if (!relatedPhrasesDeserialized.IsSuccessful) return Ok(relatedPhrasesDeserialized);

                _cacheHelper.AddCachedItem(cacheKey, responseContent);

                relatedPhrasesDeserialized.TotalPages = (int)Math.Ceiling((double)relatedPhrasesDeserialized.Data.Rows.Count / Constants.DefaultPageSize);
                relatedPhrasesDeserialized.Data.Rows = relatedPhrasesDeserialized.Data.Rows
                    .Skip((pageNumber - 1) * Constants.DefaultPageSize)
                    .Take(Constants.DefaultPageSize)
                    .ToList();

                return Ok(relatedPhrasesDeserialized);
            }

            return Ok(relatedPhrasesDto);
        }
    }
}
