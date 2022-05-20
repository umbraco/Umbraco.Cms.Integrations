using System;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Extensions;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Services;
#else
using System.Configuration;

using Umbraco.Web;
using Umbraco.Web.WebApi;
using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    /// <summary>
    /// When a Zapier user creates a "New Content Published" triggered, he is authenticated, then selects a content type, the API provides an output json with the
    /// structure of a content node matching the selected content type.
    /// For version 1.0.0 of the Umbraco Zapier App, the GetSampleContent will be used.
    /// </summary>
    public class PollingController : UmbracoApiController
    {
        private IUmbracoContextFactory _umbracoContextFactory;

        private IContentService _contentService;

        private readonly ZapierSettings Options;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        public PollingController(IOptions<ZapierSettings> options, IUmbracoContextFactory umbracoContextFactory, IContentService contentService, IUserValidationService userValidationService)
#else
        public PollingController(IUmbracoContextFactory umbracoContextFactory, IContentService contentService, IUserValidationService userValidationService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _umbracoContextFactory = umbracoContextFactory;

            _contentService = contentService;

            _userValidationService = userValidationService;
        }

        [Obsolete("Used only for Umbraco Zapier app v1.0.0. For updated versions use GetContentByType")]
        public IEnumerable<PublishedContentDto> GetSampleContent()
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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return null;

            var isAuthorized = _userValidationService.Validate(username, password, Options.UserGroup).GetAwaiter()
                .GetResult();
            if (!isAuthorized) return null;

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
            var list = new List<Dictionary<string, string>>();

            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var cache = cref.UmbracoContext.Content;
                
                var node = cache.GetByXPath($"//{alias}")
                    .Where(p => p.IsPublished())
                    .OrderByDescending(p => p.UpdateDate)
                    .FirstOrDefault();

                if (node == null) return list;

                list.Add(node.ToContentDictionary());

                return list;
            }
        }
    }
}
