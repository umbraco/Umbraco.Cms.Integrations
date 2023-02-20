using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.DAM.Aprimo;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models;
using Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels;
using Umbraco.Cms.Integrations.DAM.Aprimo.Services;

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

            var response = _assetsService.SearchRecord(Guid.Parse(vm.Id)).ConfigureAwait(false).GetAwaiter().GetResult();
            if(!response.IsAuthorized)
            {
                var tokenResponse = _authorizationService.RefreshAccessToken().ConfigureAwait(false).GetAwaiter().GetResult();
                if (string.IsNullOrEmpty(tokenResponse))
                {
                    response = _assetsService.SearchRecord(Guid.Parse(vm.Id)).ConfigureAwait(false).GetAwaiter().GetResult();
                    if(response.Success)
                    {
                        ToViewModel(response.Data, ref vm);
                    }
                }
            } else if(response.Success)
            {
                ToViewModel(response.Data, ref vm);
            }

            return vm;
        }

        private void ToViewModel(Record record, ref AprimoAssetViewModel vm)
        {
            vm.Title = record.Title;
            vm.Thumbnail = record.Thumbnail.Uri;
            vm.Tag = record.Tag;
        }
    }
}

