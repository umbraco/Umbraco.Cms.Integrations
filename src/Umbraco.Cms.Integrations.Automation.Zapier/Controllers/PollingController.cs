using System.Collections.Generic;

using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
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
    public class PollingController : UmbracoApiController
    {
        private readonly ZapierSettings Options;

        private IContentService _contentService;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        public PollingController(IOptions<ZapierSettings> options, IContentService contentService, IUserValidationService userValidationService)
#else
        public PollingController(IContentService contentService, IUserValidationService userValidationService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _contentService = contentService;

            _userValidationService = userValidationService;
        }

        public List<Dictionary<string, string>> GetPublishedContent()
        {
            var publishedContent = new List<Dictionary<string, string>>();

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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return null;

            var isAuthorized = _userValidationService.Validate(username, password, Options.UserGroup).GetAwaiter().GetResult();
            if (!isAuthorized) return null;

            var rootNodes = _contentService.GetRootContent().Where(p => p.Published)
                .OrderByDescending(p => p.PublishDate);

            foreach (var publishedContentRoot in rootNodes)
            {
                publishedContent.Add(new Dictionary<string, string>
                {
                    { Constants.Content.Id, publishedContentRoot.Id.ToString() },
                    { Constants.Content.Name, publishedContentRoot.Name },
                    { Constants.Content.PublishDate, publishedContentRoot.PublishDate.Value.ToString() }
                });

                var ancestors = _contentService.GetAncestors(publishedContentRoot);
                foreach (var ancestor in ancestors)
                {
                    publishedContent.Add(new Dictionary<string, string>
                    {
                        { Constants.Content.Id, ancestor.Id.ToString() },
                        { Constants.Content.Name, ancestor.Name },
                        { Constants.Content.PublishDate, ancestor.PublishDate.Value.ToString() }
                    });

                }
            }

            return publishedContent;
        }

    }
}
