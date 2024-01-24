using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public class RecordBuilderFactory : IRecordBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IUmbracoContextFactory _umbracoContextFactory;

        private readonly IDictionary<string, Type> _recordBuilderCache = new Dictionary<string, Type>();

        public RecordBuilderFactory(IServiceProvider serviceProvider, IUmbracoContextFactory umbracoContextFactory)
        {
            _serviceProvider = serviceProvider;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public IRecordBuilder GetRecordBuilderService(IContent content)
        {
            using var context = _umbracoContextFactory.EnsureUmbracoContext();

            var contentType = context.UmbracoContext.Content
                ?.GetById(true, content.Id)
                ?.GetType();

            if (contentType == null)
            {
                return null;
            }
            var serviceType = GetRecordBuilderType(contentType);

            return _serviceProvider.GetService(serviceType) as IRecordBuilder;
        }

        private Type GetRecordBuilderType(Type contentType)
        {
            var dictionaryKey = contentType.FullName;
            if (_recordBuilderCache.ContainsKey(dictionaryKey))
            {
                return _recordBuilderCache[dictionaryKey];
            }

            var type = typeof(IRecordBuilder<>).MakeGenericType(contentType);
            _recordBuilderCache[dictionaryKey] = type;
            return type;
        }
    }
}
