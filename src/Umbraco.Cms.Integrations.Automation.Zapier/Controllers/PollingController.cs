using System.Collections.Generic;

using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#else
using System.Configuration;

using Umbraco.Web;
using Umbraco.Web.WebApi;
using Umbraco.Core.Services;
using Umbraco.Core.Models.PublishedContent;

#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Controllers
{
    public class PollingController : UmbracoApiController
    {
        private IUmbracoContextFactory _umbracoContextFactory;

        private IContentService _contentService;

        private readonly ZapierSettings Options;

        private ZapierFormService _zapierFormService;

        private readonly IUserValidationService _userValidationService;

#if NETCOREAPP
        public PollingController(IOptions<ZapierSettings> options, IUmbracoContextFactory umbracoContextFactory, IContentService contentService, ZapierFormService zapierFormService, IUserValidationService userValidationService)
#else
        public PollingController(IUmbracoContextFactory umbracoContextFactory, IContentService contentService, ZapierFormService zapierFormService, IUserValidationService userValidationService)
#endif
        {
#if NETCOREAPP
            Options = options.Value;
#else
            Options = new ZapierSettings(ConfigurationManager.AppSettings);
#endif

            _umbracoContextFactory = umbracoContextFactory;

            _contentService = contentService;

            _zapierFormService = zapierFormService;

            _userValidationService = userValidationService;
        }

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

        public List<Dictionary<string, string>> GetContentByAlias(string contentTypeAlias)
        {
            var list = new List<Dictionary<string, string>>();

            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var cache = cref.UmbracoContext.Content;
                
                var nodes = cache.GetByXPath($"//{contentTypeAlias}")
                    .Where(p => p.IsPublished())
                    .OrderByDescending(p => p.UpdateDate);

                foreach (var node in nodes)
                {
                    var content = new Dictionary<string, string>
                    {
                        {Constants.Content.Id, node.Id.ToString() },
                        {Constants.Content.Name, node.Name },
                        {Constants.Content.PublishDate, node.UpdateDate.ToString("s") }
                    };

                    foreach (var prop in node.Properties)
                    {
                        content.Add(prop.Alias, prop.GetValue().ToString());
                    }
                    

                    list.Add(content);
                }

                return list;
            }
        }

        public IEnumerable<FormDto> GetSampleForm()
        {
            return _zapierFormService.GetAll();
        }
    }
}
