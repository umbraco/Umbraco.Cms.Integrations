namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class ResponseDto<T> 
        where T : class
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("nextPageInfo")]
        public string NextPageInfo { get; set; }

        [JsonPropertyName("previousPageInfo")]
        public string PreviousPageInfo { get; set; }

        [JsonPropertyName("isExpired")]
        public bool IsExpired { get; set; }

        [JsonPropertyName("result")]
        public T Result { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("skip")]
        public int Skip { get; set; }

        [JsonPropertyName("take")]
        public int Take { get; set; }
    }
}
