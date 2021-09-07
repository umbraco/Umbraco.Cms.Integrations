using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    internal class Category
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public Dictionary<string, string> Name { get; set; }

        public Dictionary<string, string> Description { get; set; }

        public string OrderHint { get; set; }

        public Dictionary<string, string> Slug { get; set; }
    }
}
