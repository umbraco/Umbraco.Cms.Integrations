using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common;
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
        private readonly IContentTypeService _contentTypeService;

        private readonly IZapierContentService _zapierContentService;

#if NETCOREAPP
        private readonly UmbracoHelper _umbracoHelper;

        public PollingController(
            IOptions<ZapierSettings> options, 
            IContentService contentService, 
            IContentTypeService contentTypeService, 
            UmbracoHelper umbracoHelper, 
            IUserValidationService userValidationService,
            IZapierContentService zapierContentService)
            : base(options, userValidationService)
#else
        public PollingController(
            IContentTypeService contentTypeService, 
            IUserValidationService userValidationService,
            IZapierContentService zapierContentService)
            : base(userValidationService)
#endif
        {
            _contentTypeService = contentTypeService;

            _zapierContentService = zapierContentService;

#if NETCOREAPP
            _umbracoHelper = umbracoHelper;
#endif
        }

        public List<Dictionary<string, string>> GetContentByType(string alias)
        {
            if (!IsAccessValid()) return null;

            var contentType = _contentTypeService.Get(alias);
            if (contentType == null) return new List<Dictionary<string, string>>();

#if NETCOREAPP
            var contentItems = _umbracoHelper.ContentAtXPath("//" + alias)
                .OrderByDescending(p => p.UpdateDate);
#else
            var contentItems = Umbraco.ContentAtXPath("//" + alias)
                .OrderByDescending(p => p.UpdateDate);
#endif

            return new List<Dictionary<string, string>> 
                { 
                    _zapierContentService.GetContentTypeDictionary(contentType, contentItems.FirstOrDefault()) 
                };
        }

        
    }
}
