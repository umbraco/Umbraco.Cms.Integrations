﻿using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extensions;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    /// <summary>
    /// When a Zapier user creates a "New Content Published" triggered, they are authenticated, then select a content type, the API provides an output json with the
    /// structure of a content node matching the selected content type.
    /// For version 1.0.0 of the Umbraco Zapier App, the GetSampleContent will be used.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class PollingController : ZapierControllerBase
    {
        private readonly IContentTypeService _contentTypeService;

        private readonly IZapierContentService _zapierContentService;

        private readonly UmbracoHelper _umbracoHelper;

        public PollingController(
            IOptions<ZapierSettings> options,
            IContentService contentService,
            IContentTypeService contentTypeService,
            UmbracoHelper umbracoHelper,
            IUserValidationService userValidationService,
            IZapierContentService zapierContentService)
            : base(options, userValidationService)
        {
            _contentTypeService = contentTypeService;
            _zapierContentService = zapierContentService;
            _umbracoHelper = umbracoHelper;
        }

        [HttpGet("content-by-type")]
        [ProducesResponseType(typeof(List<Dictionary<string, string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContentByType(string alias)
        {
            if (!IsAccessValid())
                return NotFound();

            var contentType = _contentTypeService.Get(alias);
            if (contentType == null)
                return Ok(new List<Dictionary<string, string>>());

            var contentItems = _umbracoHelper.ContentAtRoot().DescendantsOrSelfOfType(alias)
                .OrderByDescending(p => p.UpdateDate);
            var contentTypeDictionary = new List<Dictionary<string, string>>{
                _zapierContentService.GetContentTypeDictionary(contentType, contentItems.FirstOrDefault())
            };

            return Ok(contentTypeDictionary);
        }
    }
}
