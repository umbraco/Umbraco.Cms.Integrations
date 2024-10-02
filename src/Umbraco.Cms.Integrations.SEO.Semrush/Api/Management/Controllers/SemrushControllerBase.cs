using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
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
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        protected readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        protected static Func<HttpClient> ClientFactory = () => s_client;

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
            AuthorizationImplementationFactory authorizationImplementationFactory)
        {
            _settings = options.Value;
            _webHostEnvironment = webHostEnvironment;
            _semrushTokenService = semrushTokenService;
            _cacheHelper = cacheHelper;
            _tokenBuilder = tokenBuilder;
            _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);
        }
    }
}
