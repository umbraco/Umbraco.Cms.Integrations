using System;
using System.Text.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Editors
{
	public class FormPickerValueConverter : PropertyValueConverterBase
    {
		public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorUiAlias == "HubSpot.PropertyEditorUi.FormPicker";

		public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

		public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(HubspotFormViewModel);

		public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
		{
			if (source == null)
			{
				return null;
			}

			return JsonSerializer.Deserialize<HubspotFormViewModel>(source.ToString());
		}
	}
}

