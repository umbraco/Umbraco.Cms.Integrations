using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public interface IRecordBuilder
    {
        Record GetRecord(IContent content, Record record);
    }

    public interface IRecordBuilder<in TContentType> 
        : IRecordBuilder where TContentType : PublishedContentModel 
    {
    }
}
