
using System.Collections.Generic;

namespace Umbraco.Cms.Integrations.SEO.Semrush.Configuration
{
    public class SemrushSettings
    {
        public static int DefaultPageSize = 10;

        public static string AuthProxyBaseAddress = "https://localhost:44364/";

        public static string AuthProxyTokenEndpoint = "oauth/v1/token";

        public static string SemrushBaseAddress = "https://oauth.semrush.com/";

        public static KeyValuePair<string, string> SemrushServiceHeaderKey =
            new KeyValuePair<string, string>("service_name", "Semrush");

        public static string SemrushAuthorizationEndpoint
            = $"{SemrushBaseAddress}oauth2/authorize?ref=0053752252&client_id=umbraco&redirect_uri=%2Foauth2%2Fumbraco%2Fsuccess&response_type=code&scope=user.id,domains.info,url.info,positiontracking.info";

        public static string SemrushKeywordsEndpoint =
            SemrushBaseAddress + "api/v1/keywords/{0}?access_token={1}&phrase={2}&database={3}";

        public static string TokenDbKey = "Umbraco.Cms.Integrations.Semrush.TokenDbKey";

        public static string AllowLimitOffsetHeaderName = "Allow-Limit-Offset";
    }
}
