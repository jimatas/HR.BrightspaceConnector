namespace HR.BrightspaceConnector.Infrastructure.Hosting
{
    internal static class HostBuilderExtensions
    {
        public static IHostBuilder UseStartup<TStartup>(this IHostBuilder hostBuilder)
            where TStartup : class
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {
                var hasConfigurationConstructor = typeof(TStartup).GetConstructor(new[] { typeof(IConfiguration) }) is not null;
                var startupInstance = hasConfigurationConstructor
                    ? (TStartup?)Activator.CreateInstance(typeof(TStartup), context.Configuration)
                    : (TStartup?)Activator.CreateInstance(typeof(TStartup));

                var configureServicesMethod = typeof(TStartup).GetMethod("ConfigureServices", new[] { typeof(IServiceCollection) });
                configureServicesMethod?.Invoke(startupInstance, new[] { services });
            });
        }
    }
}
