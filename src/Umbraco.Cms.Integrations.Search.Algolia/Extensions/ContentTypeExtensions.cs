using Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos;

namespace Umbraco.Cms.Integrations.Search.Algolia.Extensions
{
    public static class ContentTypeExtensions
    {
        public static IEnumerable<ContentTypeDto> FilterByPropertySelected(this IEnumerable<ContentTypeDto> source)
        {
            foreach (var item in source)
            {
                if (!item.Selected) continue;

                var contentType = item;

                contentType.Properties = item.Properties.Where(x => x.Selected);

                if (!contentType.Properties.Any()) continue;

                yield return contentType;
            }
        }
    }
}
