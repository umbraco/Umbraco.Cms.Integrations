
namespace Umbraco.Cms.Integrations.PIM.Inriver.Models.ViewModels
{
    public class InriverEntityViewModel
    {
        public InriverEntityViewModel()
        {
            Fields = Enumerable.Empty<FieldValue>();
            Outbound = Enumerable.Empty<EntityData>();
        }

        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string DisplayDescription { get; set; }

        public string ResourceUrl { get; set; }

        public IEnumerable<FieldValue> Fields { get; set; }

        public IEnumerable<EntityData> Outbound { get; set; }
    }
}
