﻿using Newtonsoft.Json.Linq;
using System;
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

			var jObject = JObject.Parse(source.ToString());
			var jformId = jObject["id"];
			var jportalId = jObject["portalId"];
			var jRegion = jObject["region"];

			if (jformId != null && jportalId != null)
			{
				var hubspotFormViewModel = new HubspotFormViewModel
				{
					Id = jformId.Value<string>(),
					PortalId = jportalId.Value<string>(),
					Region = jRegion.Value<string>()
				};
				return hubspotFormViewModel;
			}

			return null;
		}
	}
}

