using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Library.Services;

namespace Umbraco.Cms.Integrations.Library
{
    public class LibraryComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IParserService, ParserService>();
        }
    }
}
