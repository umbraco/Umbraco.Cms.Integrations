﻿
namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels
{
    public class AprimoAssetViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;

        public AprimoMediaWithCropsViewModel MediaWithCrops { get; set; }

        public List<AprimoFieldViewModel> Fields { get; set; }

        public AprimoAssetViewModel()
        {
            MediaWithCrops = new AprimoMediaWithCropsViewModel();
            Fields = new List<AprimoFieldViewModel>();
        }
    }
}
