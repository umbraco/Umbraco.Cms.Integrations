
namespace Umbraco.Cms.Integrations.PIM.Inriver.Models.ViewModels
{
    public class InriverEntityViewModel
    {
        public InriverEntityViewModel()
        {
            Fields = new Dictionary<string, object>();
        }

        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string DisplayDescription { get; set; }

        public Dictionary<string, object> Fields { get; set; }
    }
}
