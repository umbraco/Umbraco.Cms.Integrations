using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

using Umbraco.Cms.Integrations.PIM.Inriver;
using Umbraco.Cms.Integrations.PIM.Inriver.Models.ViewModels;
using Umbraco.Cms.Integrations.PIM.Inriver.Services;

namespace Umbraco.Cms.Integrations.Pim.Inriver.Editors
{
    public class EntityPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IInriverService _inriverService;

        public EntityPickerValueConverter(IInriverService inriverService)
        {
            _inriverService = inriverService;   
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals(Constants.PropertyEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(InriverEntityViewModel);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return null;
            }

            var node = JsonNode.Parse(source.ToString());

            int entityId = (int) node["entityId"];

            var displayFields = node["displayFields"] as JsonArray;

            var entitySummary = _inriverService.GetEntitySummary(entityId).ConfigureAwait(false).GetAwaiter().GetResult();

            if (entitySummary.Failure) return null;

            var vm = new InriverEntityViewModel
            {
                DisplayName = entitySummary.Data.DisplayName,
                DisplayDescription = entitySummary.Data.Description
            };

            var entityFieldValues = _inriverService.GetEntityFieldValues(entityId)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            if (entityFieldValues.Failure) return vm;

            foreach(var displayField in displayFields)
            {
                var field = entityFieldValues.Data.First(p => p.FieldTypeId == displayField.GetValue<string>());

                vm.Fields.Add(field.FieldTypeId, field.Value);
            }

            return vm;
        }
    }
}

