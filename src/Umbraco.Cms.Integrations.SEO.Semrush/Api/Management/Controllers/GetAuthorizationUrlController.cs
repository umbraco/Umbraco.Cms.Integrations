﻿using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.SemrushGroupName)]
    public class GetAuthorizationUrlController : SemrushControllerBase
    {
        public GetAuthorizationUrlController(
            IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper,
            TokenBuilder tokenBuilder,
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
        {
        }

        [HttpGet("auth/url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetAuthorizationUrl() => Ok(_authorizationService.GetAuthorizationUrl());
    }
}