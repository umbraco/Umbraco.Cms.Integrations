using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Helpers
{
    public static class RecordHelper
    {
        public static AprimoMediaItemViewModel ToAprimoMediaItemViewModel(this RecordRenditionItem item)
        {
            string url = string.Empty;

            if (item.PublicLinks != null && item.PublicLinks.Items != null && item.PublicLinks.Items.Any())
                url = item.PublicLinks.Items.First().Uri;

            return new(item.ResizeWidth, item.ResizeHeight,
               item.Name, item.PresetName,
               item.Label, item.FileName,
               item.Extension, url);
        }
    }
}
