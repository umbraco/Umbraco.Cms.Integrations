#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

using Umbraco.Integrations.Library.Factories;
using Umbraco.Integrations.Library.Interfaces;
using Umbraco.Integrations.Library.Services;

namespace Umbraco.Integrations.Library
{
    public class LibraryComposer : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ILibraryFactory, LibraryFactory>();

            builder.Services.AddSingleton<ITokenService, TokenService>();

            builder.Services.AddSingleton<ICacheHelper, CacheHelper>();

            builder.Services.AddSingleton<ILoggingService, LoggingService>();

            builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
        }
#else
        public void Compose(Composition composition)
        {
            composition.Register<ILibraryFactory, LibraryFactory>(Lifetime.Singleton);

            composition.Register<ITokenService, TokenService>(Lifetime.Singleton);

            composition.Register<ICacheHelper, CacheHelper>(Lifetime.Singleton);

            composition.Register<ILoggingService, LoggingService>(Lifetime.Singleton);

            composition.Register<IUserValidationService, UserValidationService>(Lifetime.Singleton);
        }
#endif

    }
}
