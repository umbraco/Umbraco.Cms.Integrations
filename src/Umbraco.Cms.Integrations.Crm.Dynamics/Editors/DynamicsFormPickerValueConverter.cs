using Json.More;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.Crm.Dynamics.Helpers;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Editors
{
    public class DynamicsFormPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IDynamicsService _dynamicsService;

        public DynamicsFormPickerValueConverter(IDynamicsService dynamicsService)
        {
            _dynamicsService = dynamicsService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals("Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker");

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(FormViewModel);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;

            var jsonObj = JsonSerializer.Deserialize<JsonObject>(source.ToString());

            if (jsonObj is null) return null;

            var vm = new FormViewModel
            {
                IframeEmbedded =  jsonObj.TryGetValue("iframeEmbedded", out var _, out var _) ? (bool)jsonObj["iframeEmbedded"] : false
            };

            var module = (DynamicsModule)Enum.Parse(typeof(DynamicsModule), jsonObj["module"].ToString());

            vm.Module = module;

            if (module.HasFlag(DynamicsModule.Outbound))
            {

                var embedCode = jsonObj["embedCode"].ToString();

                vm.FormBlockId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataFormBlockId);
                vm.ContainerId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.ContainerId);
                vm.ContainerClass = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.ContainerClass);
                vm.WebsiteId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataWebsiteId);
                vm.Hostname = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataHostname);
            }
            else
            {
                var form = _dynamicsService.GetRealTimeForm(jsonObj["id"].ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                if (form != null)
                {
                    vm.Html = form.StandaloneHtml;
                }
            }

            return vm;
        }
    }
}
