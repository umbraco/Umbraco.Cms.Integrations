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
        private readonly IContentTypeService _contentTypeService;
        public GetCurrentContentPropertiesController(IOptions<SemrushSettings> options,
            IWebHostEnvironment webHostEnvironment,
            ISemrushTokenService semrushTokenService,
            ICacheHelper cacheHelper,
            TokenBuilder tokenBuilder,
            SemrushComposer.AuthorizationImplementationFactory authorizationImplementationFactory,
            IContentService contentService,
            IContentTypeService contentTypeService,
            IHttpClientFactory httpClientFactory) : base(options, webHostEnvironment, semrushTokenService, cacheHelper, tokenBuilder, authorizationImplementationFactory, httpClientFactory)
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
        }

        [HttpGet("content-properties")]
        [ProducesResponseType(typeof(List<ContentPropertyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentContentProperties(string contentId)
        {
            var propertyList = new List<ContentPropertyDto>();
            var currentContent = _contentService.GetById(Guid.Parse(contentId));
            var propertiesInCurrentContent = currentContent.Properties.ToList();
            var contentTypeOfCurrentContent = _contentTypeService.Get(currentContent.ContentType.Id);
            var contentGroupsOfCurrentContentType = contentTypeOfCurrentContent.PropertyGroups.ToList();

            for(int i = 0; i < contentGroupsOfCurrentContentType.Count; i++)
            {
                var properties = contentGroupsOfCurrentContentType[i].PropertyTypes.ToList();

                for(int j = 0; j < properties.Count; j++)
                {
                    var property = properties[j];
                    var value = propertiesInCurrentContent.Where(p => p.PropertyTypeId == property.Id).FirstOrDefault().Values;
                    if(value != null && value.Count > 0)
                    {
                        propertyList.Add(new ContentPropertyDto
                        {
                            PropertyName = property.Name,
                            PropertyGroup = contentGroupsOfCurrentContentType[i].Name,
                            PropertyValue = value.FirstOrDefault().PublishedValue.ToString()
                        });
                    }
                }
            }

            return Ok(propertyList);
        }
    }
}
