using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Library.Services
{
    public interface IParserService
    {
        string GetParsedValue(IProperty property, string culture = "");

        string GetParsedValue(IPublishedProperty property, string culture = "");
    }
}
