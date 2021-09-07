using Umbraco.Core;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models.Search.Filters
{
    public class FieldFilter : BaseFilter
    {
        public string Value { get; }

        public string FieldName { get; }

        /// <summary>
        /// Filter for a specific field.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="languageCode"></param>
        public FieldFilter(string fieldName, string value, string languageCode = "")
        {
            FieldName = fieldName;
            Value = value;
            if (!languageCode.IsNullOrWhiteSpace())
            {
                FieldName += "." + languageCode;
            }
        }

        public override string Stringify()
        {
            return $"{FieldName}=\"{Value}\"";
        }
    }
}
