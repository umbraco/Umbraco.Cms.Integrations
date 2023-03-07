
namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels
{
    public class AprimoAssetViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;

        public IEnumerable<AprimoCropItemViewModel> CropItemsVM { get; set; }

        public List<AprimoFieldViewModel> FieldsVM { get; set; }

        public AprimoAssetViewModel()
        {
            CropItemsVM = Enumerable.Empty<AprimoCropItemViewModel>();
            FieldsVM = new List<AprimoFieldViewModel>();
        }
    }
}
