using System.Text.Json;

using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Integrations.Library.Parsing
{
    public class MediaWithCropsParser : BaseParser<MediaWithCrops>
    {
        public override string Parse(MediaWithCrops input)
        {
            if(input.LocalCrops != null && input.LocalCrops.Src != null)
                return input.LocalCrops.Src;

            return string.Empty;
        }

        public override string Parse(List<MediaWithCrops> input)
        {
            if(input.Count() > 0)
            {
                var srcArr = input
                    .Where(p => p.LocalCrops != null && p.LocalCrops.Src != null)
                    .Select(p => p.LocalCrops.Src)
                    .ToArray();

                return JsonSerializer.Serialize(srcArr);
            }

            return string.Empty;
        }
    }
}
