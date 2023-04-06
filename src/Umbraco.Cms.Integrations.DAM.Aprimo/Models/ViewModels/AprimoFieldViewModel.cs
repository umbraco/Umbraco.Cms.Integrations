
namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels
{
    public class AprimoFieldViewModel
    {
        public string Label { get; set; }

        public Dictionary<string, string> Values { get; set; }

        public AprimoFieldViewModel()
        {
            Values = new Dictionary<string, string>();
        }
    }
}
