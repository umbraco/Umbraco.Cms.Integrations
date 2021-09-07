using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Filters
{
    public class CategoryNameFilter : BaseFilter
    {
        public string Name { get; }

        public string LanguageCode { get; }

        /// <summary>
        /// Filter for a specific name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="languageCode"></param>
        public CategoryNameFilter(string name, string languageCode)
        {
            Name = name;
            LanguageCode = languageCode;
        }

        public override string Stringify()
        {
            return $"name({LanguageCode}=\"{Name}\")";
        }
    }
}
