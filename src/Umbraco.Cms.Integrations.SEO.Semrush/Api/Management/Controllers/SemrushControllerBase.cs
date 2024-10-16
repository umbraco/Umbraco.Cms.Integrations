using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using static Umbraco.Cms.Integrations.SEO.Semrush.SemrushComposer;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ManagementApi.ApiName)]
    public class SemrushControllerBase : Controller
    {
        protected static IHttpClientFactory _clientFactory;
        private IOptions<SemrushSettings> options;
        private IWebHostEnvironment webHostEnvironment;
        private ISemrushTokenService semrushTokenService;
        private ICacheHelper cacheHelper;
        private TokenBuilder tokenBuilder;
        private AuthorizationImplementationFactory authorizationImplementationFactory;
        protected static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        protected readonly ISemrushTokenService _semrushTokenService;

        protected readonly ICacheHelper _cacheHelper;

        protected readonly TokenBuilder _tokenBuilder;

        protected readonly SemrushSettings _settings;

        protected readonly ISemrushAuthorizationService _authorizationService;

        protected readonly IWebHostEnvironment _webHostEnvironment;

        public SemrushControllerBase(IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper, TokenBuilder tokenBuilder,
            AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory clientFactory)
        {
            _settings = options.Value;
            _webHostEnvironment = webHostEnvironment;
            _semrushTokenService = semrushTokenService;
            _cacheHelper = cacheHelper;
            _tokenBuilder = tokenBuilder;
            _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);
            _clientFactory = clientFactory;
        }
    }
}
