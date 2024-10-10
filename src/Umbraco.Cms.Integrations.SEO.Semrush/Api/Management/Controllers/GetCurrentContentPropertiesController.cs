using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.SEO.Semrush.Configuration;
using Umbraco.Cms.Integrations.SEO.Semrush.Models.Dtos;
using Umbraco.Cms.Integrations.SEO.Semrush.Services;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.SemrushGroupName)]
    public class GetCurrentContentPropertiesController : SemrushControllerBase
    {
        private readonly IContentService _contentService;
        public GetCurrentContentPropertiesController(IOptions<SemrushSettings> options, 
            IWebHostEnvironment webHostEnvironment, 
            ISemrushTokenService semrushTokenService, 
            ICacheHelper cacheHelper, 
            TokenBuilder tokenBuilder, 
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IContentService contentService) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory)
        {
            _contentService = contentService;
        }

        [HttpGet("content-properties")]
        [ProducesResponseType(typeof(List<ContentPropertyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentContentProperties(string contentId)
        {
            var propertyList = new List<ContentPropertyDto>();

            var content = _contentService.GetById(Guid.Parse(contentId));
            if (content != null)
            {
                propertyList = content.Properties.Select(s => new ContentPropertyDto { PropertyName = s.PropertyType.Name, PropertyValue = s.Values.ToList()[0].PublishedValue.ToString() }).ToList();
            }
            
            return Ok(propertyList);
        }
    }
}
