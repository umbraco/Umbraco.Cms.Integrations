
namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos
{
    public class ResponseDto<T>
        where T : class
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }
    }
}
