using System;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search
{
    public class IdsRequest
    {
        public Guid[] Ids { get; set; }

        public string LanguageCode { get; set; } = null;
    }
}
