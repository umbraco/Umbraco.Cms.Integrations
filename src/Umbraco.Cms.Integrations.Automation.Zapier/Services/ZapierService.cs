using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierService
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        public string TriggerAsync(string requestUri, Dictionary<string, string> content)
        {
            var result = ClientFactory().PostAsync(requestUri, new FormUrlEncodedContent(content)).GetAwaiter().GetResult();

            return result.IsSuccessStatusCode ? string.Empty : result.ReasonPhrase;
        }
    }
}
