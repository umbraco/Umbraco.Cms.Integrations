using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Helpers
{
    public static class RecordHelper
    {
        public static AprimoCropItemViewModel ToAprimoCropItemViewModel(this RecordFileItem item) => new(item.X, item.Y,
               item.Width, item.Height,
               item.ResizeWidth, item.ResizeHeight,
               item.PresetName, item.Label, item.FileName,
               item.PublicLink);
    }
}
