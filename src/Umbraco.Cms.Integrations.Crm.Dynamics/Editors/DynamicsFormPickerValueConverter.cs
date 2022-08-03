using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Umbraco.Cms.Integrations.Crm.Dynamics.Helpers;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels;

#if NETCOREAPP
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Editors
{
    public class DynamicsFormPickerValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals("Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker");

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(FormViewModel);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;

            var jObject = JObject.Parse(source.ToString());

            var embedCode = jObject["embedCode"].ToString();
            var iFrameEmbedd = (bool)jObject["iFrameEmbedded"];

            var vm = new FormViewModel
            {
                IFrameEmbedded = iFrameEmbedd,
                FormBlockId = embedCode.ParseAttributeValue(Constants.EmbedAttribute.DataFormBlockId),
                ContainerId = embedCode.ParseAttributeValue(Constants.EmbedAttribute.ContainerId),
                ContainerClass = embedCode.ParseAttributeValue(Constants.EmbedAttribute.ContainerClass),
                WebsiteId = embedCode.ParseAttributeValue(Constants.EmbedAttribute.DataWebsiteId),
                Hostname = embedCode.ParseAttributeValue(Constants.EmbedAttribute.DataHostname)
            };

            return vm;
        }
    }
}
