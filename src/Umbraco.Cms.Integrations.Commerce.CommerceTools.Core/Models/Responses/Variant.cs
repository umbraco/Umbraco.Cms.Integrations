using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    internal class Variant
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string SKU { get; set; }

        public IEnumerable<VariantAttribute> Attributes { get; set; }

        public IEnumerable<Image> Images { get; set; }

        public IEnumerable<Price> Prices { get; set; }

        public Availability Availability { get; set; }
    }
}
