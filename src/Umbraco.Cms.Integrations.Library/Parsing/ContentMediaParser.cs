using System.Text.Json;

using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Library.Entities;

namespace Umbraco.Cms.Integrations.Library.Parsing
{
    public class ContentMediaParser : BaseParser<string>
    {
        private IMediaService _mediaService;

        public ContentMediaParser(IMediaService mediaService)
        {
            _mediaService = mediaService;   
        }

        public override string Parse(string input)
        {
            var list = new List<string>();  

            var inputMedia = JsonSerializer.Deserialize<IEnumerable<LibraryMedia>>(input);

            if (inputMedia == null) return string.Empty;

            foreach(var item in inputMedia)
            {
                if (item == null) continue;

                var mediaItem = _mediaService.GetById(Guid.Parse(item.MediaKey));

                if(mediaItem == null) continue;

                list.Add(mediaItem.GetValue("umbracoFile")?.ToString() ?? string.Empty);
            }

            return JsonSerializer.Serialize(list);
        }

        public override string Parse(List<string> input)
        {
            throw new NotImplementedException();
        }
    }
}
