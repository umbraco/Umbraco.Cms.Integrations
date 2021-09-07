using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Responses
{
    internal class Response<T>
    {
        public int Count { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public IEnumerable<T> Results { get; set; }

        public int Total { get; set; }
    }
}
