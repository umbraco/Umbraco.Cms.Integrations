
namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels
{
    public class AprimoAssetViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;

        public IEnumerable<AprimoCropViewModel> Crops { get; set; }

        public List<AprimoFieldViewModel> Fields { get; set; }

        public AprimoAssetViewModel()
        {
            Crops = Enumerable.Empty<AprimoCropViewModel>();
            Fields = new List<AprimoFieldViewModel>();
        }
    }
}
