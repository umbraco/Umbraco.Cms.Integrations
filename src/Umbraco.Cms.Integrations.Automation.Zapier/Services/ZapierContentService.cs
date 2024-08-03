using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierContentService : IZapierContentService
    {
        private readonly IZapierContentFactory _zapierContentFactory;

        public ZapierContentService(IZapierContentFactory zapierContentFactory) =>
            _zapierContentFactory = zapierContentFactory;

        public Dictionary<string, string> GetContentTypeDictionary(IContentType contentType, IPublishedContent content)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, content != null ? content.Id.ToString() : "1" },
                {Constants.ContentProperties.Name, content != null ? content.Name : contentType.Name },
                {Constants.ContentProperties.PublishDate, content != null ? content.UpdateDate.ToString("s") : DateTime.UtcNow.ToString("s") }
            };

            foreach (var propertyType in contentType.PropertyTypes)
            {
                if (content == null)
                {
                    contentDict.Add(propertyType.Alias, string.Empty);
                    continue;
                }

                var contentProperty = content.Properties.First(p => p.Alias == propertyType.Alias);

                var parser = _zapierContentFactory.Create(contentProperty.PropertyType.EditorAlias);

                contentDict.Add(propertyType.Alias, parser.GetValue(contentProperty));
            }

            return contentDict;
        }

        public Dictionary<string, string> GetContentDictionary(IContent contentNode)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, contentNode.Id.ToString() },
                {Constants.ContentProperties.Name, contentNode.Name },
                {Constants.ContentProperties.PublishDate, contentNode.UpdateDate.ToString("s") }
            };

            foreach (var prop in contentNode.Properties)
            {
                contentDict.Add(prop.Alias, prop.Id == 0 || prop.Values.Count == 0 ? string.Empty : prop.GetValue().ToString());
            }

            return contentDict;
        }
    }
}
