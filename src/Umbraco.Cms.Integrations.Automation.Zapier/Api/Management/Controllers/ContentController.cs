﻿using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Management.Controllers
{
    /// <summary>
    /// When a Zapier user creates a new "New Content Published" trigger, the API is used to provide the list of content types for handling "Published" event.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class ContentController : ZapierControllerBase
    {
        private readonly IContentTypeService _contentTypeService;
        public ContentController(IOptions<ZapierSettings> options, IContentTypeService contentTypeService, IUserValidationService userValidationService)
            : base(options, userValidationService)
        {
            _contentTypeService = contentTypeService;
        }

        [HttpGet("content-types")]
        [ProducesResponseType(typeof(IEnumerable<ContentTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContentTypes()
        {
            if (!IsAccessValid()) 
                return NotFound();

            var contentTypes = _contentTypeService.GetAll();
            var mapToDto = contentTypes
                .Select(q => new ContentTypeDto
                {
                    Id = q.Id,
                    Alias = q.Alias,
                    Name = q.Name
                });

            return Ok(mapToDto);
        }

    }
}
