using System.Collections.Generic;

using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Services;
#else
using System.Configuration;

using Umbraco.Web.WebApi;
using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// When a Zapier user creates a new "New Content Published" trigger, the API is used to provide him with the list of content types for handling "Published" event.
    /// </summary>
    public class ContentController : UmbracoApiController
    {
        private readonly ZapierSettings Options;

        private readonly IContentTypeService _contentTypeService;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        public ContentController(IOptions<ZapierSettings> options, IContentTypeService contentTypeService, IUserValidationService userValidationService)
#else
        public ContentController(IContentTypeService contentTypeService, IUserValidationService userValidationService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _contentTypeService = contentTypeService;

            _userValidationService = userValidationService;
        }

        public IEnumerable<ContentTypeDto> GetContentTypes()
        {
            string username = string.Empty;
            string password = string.Empty;

#if NETCOREAPP
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValue(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
#else
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.UsernameHeaderKey,
                    out var usernameValues))
                username = usernameValues.First();
            if (Request.Headers.TryGetValues(Constants.ZapierAppConfiguration.PasswordHeaderKey,
                    out var passwordValues))
                password = passwordValues.First();
#endif

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return Enumerable.Empty<ContentTypeDto>();

            var isAuthorized = _userValidationService.Validate(username, password, Options.UserGroup).GetAwaiter().GetResult();
            if (!isAuthorized) return Enumerable.Empty<ContentTypeDto>();

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
