using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.DAM.Aprimo;
using Umbraco.Cms.Integrations.DAM.Aprimo.Helpers;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Dam.Aprimo.Editors
{
    public class AssetPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IAprimoAuthorizationService _authorizationService;

        private readonly IAprimoService _assetsService;

        private readonly ILogger<AssetPickerValueConverter> _logger;

        public AssetPickerValueConverter(
            IAprimoAuthorizationService authorizationService, 
            IAprimoService assetsService,
            ILogger<AssetPickerValueConverter> logger)
        {
            _authorizationService = authorizationService;

            _assetsService = assetsService;

            _logger = logger;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals(Constants.PropertyEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(AprimoAssetViewModel);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return null;
            }

            var vm = new AprimoAssetViewModel
            {
                Id = source.ToString()
            };

            var response = _assetsService.GetRecordById(Guid.Parse(vm.Id));
            if(!response.IsAuthorized)
            {
                var tokenResponse = _authorizationService.RefreshAccessToken();
                if (string.IsNullOrEmpty(tokenResponse))
                {
                    response = _assetsService.GetRecordById(Guid.Parse(vm.Id));
                    if (response.Success)
                    {
                        ToViewModel(response.Data, ref vm);
                    }
                }
                else
                    _logger.LogError($"Failed to refresh access token: {tokenResponse}");
            } else if(response.Success)
            {
                ToViewModel(response.Data, ref vm);
            }

            return vm;
        }

        private void ToViewModel(Record record, ref AprimoAssetViewModel vm)
        {
            var languages = _assetsService.GetLanguages();

            vm.Title = record.Title;
            vm.Thumbnail = record.Thumbnail.Uri;

            if(record.MasterFileLatestVersion != null 
                && record.MasterFileLatestVersion.Renditions!= null
                && record.MasterFileLatestVersion.Renditions.Items != null)
            {
                var originalItem = record.MasterFileLatestVersion.Renditions.Items
                    .FirstOrDefault(p => p.Type == "Original");
                if(originalItem != null)
                {
                    vm.MediaWithCrops.Original = originalItem.ToAprimoMediaItemViewModel();
                }

                vm.MediaWithCrops.Crops = record.MasterFileLatestVersion.Renditions.Items
                    .Where(p => p.Type == "Crop")
                    .Select(p => p.ToAprimoMediaItemViewModel());
            }

            if(record.Fields != null 
                && record.Fields.Items != null 
                && record.Fields.Items.Any())
            {
                foreach(var fieldItem in record.Fields.Items)
                {
                    var fieldVM = new AprimoFieldViewModel
                    {
                        Label = fieldItem.Label
                    };

                    foreach(var item in fieldItem.Values) 
                    {
                        var lang = languages.Data.Items.FirstOrDefault(p => p.Id == item.LanguageId);
                        fieldVM.Values.Add(lang != null ? lang.Culture : "-",
                            string.IsNullOrEmpty(item.Value)
                                ? (item.Values != null ? string.Join(",", item.Values) : string.Empty)
                                : item.Value);
                    }

                    vm.Fields.Add(fieldVM);
                }
            }
        }
    }
}

