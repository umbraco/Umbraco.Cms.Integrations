using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models
{
    [DataContract]
    public class PagedResults<T>
        where T : BaseModel
    {
        [DataMember(Name = "results")]
        public IEnumerable<T> Results { get; }

        [DataMember(Name = "pageIndex")]
        public int? PageIndex { get; }

        [DataMember(Name = "pageSize")]
        public int? PageSize { get; }

        [DataMember(Name = "totalPages")]
        public int? TotalPages { get; }

        [DataMember(Name = "resultCount")]
        public int ResultCount => Results.Count();

        [DataMember(Name = "totalResultCount")]
        public int TotalResultCount { get; }

        public PagedResults(IEnumerable<T> results, int? pageIndex, int? pageSize, int totalResultCount)
        {
            Results = results ?? new T[0];
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = totalResultCount / pageSize;
            TotalResultCount = totalResultCount;
        }
    }
}
