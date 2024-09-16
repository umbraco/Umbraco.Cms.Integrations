using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class SimpleMultinodeTreePickerIndexValueConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.MultiNodeTreePicker;

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IContentService _contentService;


        public SimpleMultinodeTreePickerIndexValueConverter(
            IUmbracoContextAccessor umbracoContextAccessor,
            IContentService contentService)
        {
            _umbracoContextAccessor = umbracoContextAccessor ?? throw new ArgumentNullException(nameof(umbracoContextAccessor));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
        }


        public object ParseIndexValues(IProperty property)
        {

            if (property.Values.Count == 0)
                return Enumerable.Empty<string>();

            var value = property.GetValue()?.ToString();

            if (string.IsNullOrEmpty(value))
                return Enumerable.Empty<string>();

            var udiStrings = value.Split(',');

            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();

            var toReturn = new List<string>();

            foreach (var udiString in udiStrings)
            {
                var cacheName = umbracoContext.Content.GetById(UdiParser.Parse(udiString))?.Name;
                
                if (cacheName != null)
                {
                    toReturn.Add(cacheName);
                    continue;
                }

                else
                {
                    var guid = new GuidUdi(new Uri(udiString)).Guid;
                    var dbName = _contentService.GetById(guid)?.Name;

                    if (dbName != null)
                    {
                        toReturn.Add(dbName);
                    }
                }
            }
            return toReturn;
        }

    }
}
