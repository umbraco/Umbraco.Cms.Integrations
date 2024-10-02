using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers.Token
{
    public class ValidateTokenController : TokenControllerBase
    {
        public ValidateTokenController(IOptions<SemrushSettings> options, IWebHostEnvironment webHostEnvironment, ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper, TokenBuilder tokenBuilder, SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory)
        {
        }

        [HttpGet("validate")]
        [ProducesResponseType(typeof(AuthorizationResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateToken()
        {
            _semrushTokenService.TryGetParameters(Constants.TokenDbKey, out TokenDto token);

            if (!token.IsAccessTokenAvailable) return Ok(new AuthorizationResponseDto { IsAuthorized = false });

            var response = await ClientFactory()
                .GetAsync(string.Format(Constants.SemrushKeywordsEndpoint, _settings.BaseUrl, "phrase_related", token.AccessToken, "ping", "us"));

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _authorizationService.RefreshAccessTokenAsync();
            }

            return Ok(new AuthorizationResponseDto
            {
                IsAuthorized = response.StatusCode == HttpStatusCode.OK,
                IsValid = response.StatusCode != HttpStatusCode.Unauthorized,
                IsFreeAccount = response.Headers.TryGetValues(Constants.AllowLimitOffsetHeaderName,
                    out IEnumerable<string> values)
                    ? values.First().Equals("0")
                    : null
            });
        }
    }
}
