using System.Collections.Generic;
using System.Linq;

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
    /// When a Zapier user creates a new "New Content Published" trigger, the API is used to provide the list of content types for handling "Published" event.
    /// </summary>
    public class ContentController : ZapierAuthorizedApiController
    {
        private readonly IContentTypeService _contentTypeService;

#if NETCOREAPP
        public ContentController(IOptions<ZapierSettings> options, IContentTypeService contentTypeService, IUserValidationService userValidationService)
            : base(options, userValidationService)
#else
        public ContentController(IContentTypeService contentTypeService, IUserValidationService userValidationService)
            : base(userValidationService)
#endif
        {
            _contentTypeService = contentTypeService;
        }

        public IEnumerable<ContentTypeDto> GetContentTypes()
        {
            if (!IsAccessValid()) return null;

            var contentTypes = _contentTypeService.GetAll();

            return contentTypes
                .Select(q => new ContentTypeDto
                {
                    Id = q.Id,
                    Alias = q.Alias,
                    Name = q.Name
                });
        }

    }
}
