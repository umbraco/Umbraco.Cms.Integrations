using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    internal class ProductProjection
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public Dictionary<string, string> Name { get; set; }

        public Variant MasterVariant { get; set; }

        public IEnumerable<Variant> Variants { get; set; }
    }
}
