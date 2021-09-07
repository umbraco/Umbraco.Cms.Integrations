using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    internal class ProductData
    {
        public Variant MasterVariant { get; set; }

        public IEnumerable<Variant> Variants { get; set; }

        public Dictionary<string, string> Name { get; set; }

        public Dictionary<string, string> Slug { get; set; }

        public Dictionary<string, string> MetaTitle { get; set; }

        public Dictionary<string, string> MetaDescription { get; set; }

        public Dictionary<string, string> MetaKeywords { get; set; }

        public IEnumerable<Reference> References { get; set; }
    }
}
