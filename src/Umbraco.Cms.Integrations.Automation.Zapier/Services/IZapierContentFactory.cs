using Umbraco.Cms.Core.Models.PublishedContent;

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
