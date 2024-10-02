using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers.Token
{
    public class GetTokenDetailsController : TokenControllerBase
    {
        public GetTokenDetailsController(IOptions<SemrushSettings> options, IWebHostEnvironment webHostEnvironment, ISemrushTokenService semrushTokenService, ICacheHelper cacheHelper, TokenBuilder tokenBuilder, SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory)
        {
        }

        [HttpGet("detail")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        public IActionResult GetTokenDetails()
        {
            var result = _semrushTokenService.TryGetParameters(Constants.TokenDbKey, out TokenDto tokenDto) ? tokenDto : new TokenDto();
            return Ok(result);
        }
    }
}
