using HR.BrightspaceConnector.Infrastructure;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;

namespace HR.BrightspaceConnector.Tests
{
    public abstract class DependencyInjectedTestsBase : IDisposable
    {
        private IServiceProvider? serviceProvider;

        protected IServiceProvider CreateServiceProvider(IDatabase database, IApiClient apiClient)
        {
            var services = new ServiceCollection();
            services.AddScoped(_ => database);
            services.AddScoped(_ => apiClient);
            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConsole();
            });
            services.Configure<RecoverySettings>(RecoverySettings.Names.CommandTimeoutExpired, settings =>
            {
                settings.RetryAttempts = 4;
                settings.RetryDelay = TimeSpan.FromSeconds(1);
                settings.BackOffRate = 2.0;
            });
            services.AddDispatcher();
            services.AddHandlersFromAssembly(typeof(Worker).Assembly);
            services.AddHandlersFromAssembly(GetType().Assembly);

            serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        public void Dispose()
        {
            (serviceProvider as IDisposable)?.Dispose();
            serviceProvider = null;

            GC.SuppressFinalize(this);
        }
    }
}
