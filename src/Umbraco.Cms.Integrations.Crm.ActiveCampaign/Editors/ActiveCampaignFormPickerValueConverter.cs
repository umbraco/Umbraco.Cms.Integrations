using Microsoft.Extensions.Options;

using System;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Editors
{
    public class ActiveCampaignFormPickerValueConverter : PropertyValueConverterBase
    {
        private readonly ActiveCampaignSettings _settings;

        public ActiveCampaignFormPickerValueConverter(IOptions<ActiveCampaignSettings> options)
        {
            _settings = options.Value;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => 
            propertyType.EditorAlias.Equals(Constants.PropertyEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(FormViewModel);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;

            return new FormViewModel
            {
                Id = source.ToString(),
                Account = _settings.BaseUrl.Substring(0, _settings.BaseUrl.IndexOf(".")).Replace("https://", string.Empty)
            };
        }
    }
}
