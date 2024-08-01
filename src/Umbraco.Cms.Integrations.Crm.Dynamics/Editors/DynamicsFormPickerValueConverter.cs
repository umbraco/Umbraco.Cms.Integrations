using Umbraco.Cms.Integrations.Crm.Dynamics.Helpers;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;



using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Models.PublishedContent;

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

            var vm = JsonSerializer.Deserialize<FormViewModel>(source.ToString());

            var module = (DynamicsModule)Enum.Parse(typeof(DynamicsModule), vm.ModuleStr);

            vm.Module = module;

            if (module.HasFlag(DynamicsModule.Outbound))
            {
                var embedCode = vm.EmbedCode;

                vm.FormBlockId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataFormBlockId);
                vm.ContainerId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.ContainerId);
                vm.ContainerClass = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.ContainerClass);
                vm.WebsiteId = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataWebsiteId);
                vm.Hostname = embedCode.ParseDynamicsEmbedCodeAttributeValue(Constants.EmbedAttribute.DataHostname);
            }
            else
            {
                var form = _dynamicsService.GetRealTimeForm(vm.Id).ConfigureAwait(false).GetAwaiter().GetResult();
                if (form != null)
                {
                    vm.Html = form.StandaloneHtml;
                }
            }

            return vm;
        }
    }
}
