using HR.BrightspaceConnector.Infrastructure.Hosting;
using HR.Common.Utilities;

await Host.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
    .Build()
    .RunAsync().WithoutCapturingContext();