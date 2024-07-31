namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.Dtos
{
    public class RequestDto
    {
        [JsonPropertyName("ids")]
        public long[] Ids { get; set; }
    }
}
