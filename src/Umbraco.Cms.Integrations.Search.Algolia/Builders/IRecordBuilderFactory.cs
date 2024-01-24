using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public interface IRecordBuilderFactory
    {
        IRecordBuilder GetRecordBuilderService(IContent content);
    }
}
