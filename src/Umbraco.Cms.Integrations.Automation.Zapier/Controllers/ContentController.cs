using System;
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
    public class ContentController : UmbracoApiController
    {
        private readonly ZapierSettings Options;

        private readonly IContentTypeService _contentTypeService;

        private readonly IUserValidationService _userValidationService;

        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

#if NETCOREAPP
        public ContentController(IOptions<ZapierSettings> options, IContentTypeService contentTypeService, IUserValidationService userValidationService, ZapierSubscriptionHookService zapierSubscriptionHookService)
#else
        public ContentController(IContentTypeService contentTypeService, IUserValidationService userValidationService, ZapierSubscriptionHookService zapierSubscriptionHookService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _contentTypeService = contentTypeService;

            _userValidationService = userValidationService;

            _zapierSubscriptionHookService = zapierSubscriptionHookService;
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
                .Where(p => !_zapierSubscriptionHookService.TryGetByAlias(p.Alias, out var contentConfig))
                .Select(q => new ContentTypeDto
                {
                    Id = q.Id,
                    Alias = q.Alias,
                    Name = q.Name
                });
        }

    }
}
