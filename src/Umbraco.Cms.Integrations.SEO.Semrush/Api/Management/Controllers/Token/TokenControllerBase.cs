using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers.Token
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.TokenName)]
    public class TokenControllerBase : SemrushControllerBase
    {
        public TokenControllerBase(
            IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper,
            TokenBuilder tokenBuilder,
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
        {
        }
    }
}
