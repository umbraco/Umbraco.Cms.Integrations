using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Extensions
{
    public static class ConverterExtensions
    {
        public static bool TryGetPropertyIndexValue(this IProperty property, out string value)
        {
            bool success = true;
            value = string.Empty;

            if (property.GetValue() is null || string.IsNullOrEmpty(property.GetValue().ToString()))
            {
                success = false;
                return success;
            }

            value = property.GetValue().ToString();

            return success;
        }
    }
}
