using System;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Extensions;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
#else
using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// When a Zapier user creates a "New Content Published" triggered, they are authenticated, then select a content type, the API provides an output json with the
    /// structure of a content node matching the selected content type.
    /// For version 1.0.0 of the Umbraco Zapier App, the GetSampleContent will be used.
    /// </summary>
    public class PollingController : ZapierAuthorizedApiController
    {
        private IContentService _contentService;

        private IContentTypeService _contentTypeService;

#if NETCOREAPP
        public PollingController(IOptions<ZapierSettings> options, IContentService contentService, IContentTypeService contentTypeService, IUserValidationService userValidationService)
            : base(options, userValidationService)
#else
        public PollingController(IContentService contentService, IContentTypeService contentTypeService, IUserValidationService userValidationService)
            : base(userValidationService)
#endif
        {
            _contentService = contentService;

            _contentTypeService = contentTypeService;
        }

        [Obsolete("Used only for Umbraco Zapier app v1.0.0. For updated versions use GetContentByType")]
        public IEnumerable<PublishedContentDto> GetSampleContent()
        {
            if (!IsUserValid()) return null;

            var rootNodes = _contentService.GetRootContent().Where(p => p.Published)
            .OrderByDescending(p => p.PublishDate);

            return rootNodes.Select(p => new PublishedContentDto
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                PublishDate = p.PublishDate.Value.ToString()
            });
        }

        public List<Dictionary<string, string>> GetContentByType(string alias)
        {
            if (!IsUserValid()) return null;

            var contentType = _contentTypeService.Get(alias);

            return new List<Dictionary<string, string>> { contentType.ToContentTypeDictionary() };
        }

        
    }
}
