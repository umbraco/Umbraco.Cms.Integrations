using Umbraco.Integrations.Library.Interfaces;

#if NETCOREAPP
using Microsoft.Extensions.Logging;
#else
using Umbraco.Core.Logging;
#endif

namespace Umbraco.Integrations.Library.Services
{
    public class LoggingService : ILoggingService
    {
#if NETCOREAPP
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
#else
        private readonly ILogger _logger;

        public LoggingService (ILogger logger)
#endif

        {
            _logger = logger;
        }

        public void LogError(string message)
        {
#if NETCOREAPP
            _logger.LogError(message);
#else
            _logger.Error<LoggingService>(message);
#endif
        }
    }
}
