using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Umbraco.Cms.Integrations.Crm.Dynamics.Helpers;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;



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
        private readonly DynamicsService _dynamicsService;

        public DynamicsFormPickerValueConverter(DynamicsService dynamicsService)
        {
            _dynamicsService = dynamicsService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals("Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker");

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(FormViewModel);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;

            var jObject = JObject.Parse(source.ToString());

            var vm = new FormViewModel();

            var module = (DynamicsModule)Enum.Parse(typeof(DynamicsModule), jObject["module"].ToString());
            if (module == DynamicsModule.Outbound)
            {

                var embedCode = jObject["embedCode"].ToString();
                var iframeEmbedd = (bool)jObject["iframeEmbedded"];

                vm = new FormViewModel
                {
                    IframeEmbedded = iframeEmbedd,
                    FormBlockId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataFormBlockId),
                    ContainerId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.ContainerId),
                    ContainerClass = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.ContainerClass),
                    WebsiteId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataWebsiteId),
                    Hostname = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataHostname)
                };
            }
            else
            {
                var form = _dynamicsService.GetRealTimeForm(jObject["id"].ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                vm.RawHtml = form.RawHtml;
            }

            return vm;
        }
    }
}
