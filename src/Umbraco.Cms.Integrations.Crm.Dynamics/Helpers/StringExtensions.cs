using System.Text.RegularExpressions;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Read <div></div> tags attributes like id, class, data- from input HTML using Regular Expressions.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="attributeKey"></param>
        /// <returns></returns>
        public static string ParseDynamicsEmbedCodeAttributeValue(this string html, string attributeKey)
        {
            string divPattern = @"<\s*{0}[^>]*>(.*?)<\s*/{0}\s*>";
            string attributePattern = @"{0}=""([^""]*)""";

            MatchCollection divs = Regex.Matches(html, string.Format(divPattern, "div"));

            foreach (Match divMatch in divs)
            {
                if (Regex.IsMatch(divMatch.Value, string.Format(attributePattern, attributeKey)))
                {
                    string attribute = Regex.Match(divMatch.Value, string.Format(attributePattern, attributeKey)).Value;

                    return string.IsNullOrEmpty(attribute)
                        ? string.Empty
                        : attribute.Substring(attributeKey.Length + 1).Replace("\"", string.Empty);
                }
            }

            return string.Empty;
        }

    }
}
