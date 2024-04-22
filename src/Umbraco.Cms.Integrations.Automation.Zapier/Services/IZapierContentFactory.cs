#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public interface IZapierContentFactory
    {
        IZapierContentParser Create(string editorAlias);
    }

    public interface IZapierContentParser
    {
        string GetValue(IPublishedProperty contentProperty);
    }
}
