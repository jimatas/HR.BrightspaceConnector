using HR.BrightspaceConnector;
using HR.BrightspaceConnector.Infrastructure;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.BrightspaceConnector.Security;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Extensions.Http;

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<Worker>();
        services.AddDispatcher().AddHandlersFromAssembly(GetType().Assembly);

        services.AddScoped<IDatabase, Database>();
        services.AddDbContext<BrightspaceDbContext>(dbContext => dbContext.UseSqlServer(Configuration.GetConnectionString(nameof(BrightspaceDbContext))));

        services.Configure<JsonSerializerOptions>(jsonOptions =>
        {
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            jsonOptions.AllowTrailingCommas = true;
            jsonOptions.PropertyNameCaseInsensitive = true;
            jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        });

        services.Configure<BatchSettings>(Configuration.GetSection(nameof(BatchSettings))).PostConfigure<BatchSettings>(batchSettings =>
        {
            Validator.ValidateObject(batchSettings, new ValidationContext(batchSettings), validateAllProperties: true);
        });

        services.Configure<ApiClientSettings>(Configuration.GetSection(nameof(ApiSettings))).PostConfigure<ApiClientSettings>(apiSettings =>
        {
            Validator.ValidateObject(apiSettings, new ValidationContext(apiSettings), validateAllProperties: true);
        });
        services.AddSingleton<IOptions<ApiSettings>>(serviceProvider => serviceProvider.GetRequiredService<IOptions<ApiClientSettings>>());

        services.Configure<OAuthSettings>(Configuration.GetSection(nameof(OAuthSettings))).PostConfigure<OAuthSettings>(oAuthSettings =>
        {
            Validator.ValidateObject(oAuthSettings, new ValidationContext(oAuthSettings), validateAllProperties: true);
        });

        services.Configure<RecoverySettings>(RecoverySettings.Names.CommandTimeoutExpired, Configuration.GetSection($"{nameof(RecoverySettings)}:{RecoverySettings.Names.CommandTimeoutExpired}"))
            .PostConfigure<RecoverySettings>(RecoverySettings.Names.CommandTimeoutExpired, recoverySettings =>
        {
            Validator.ValidateObject(recoverySettings, new ValidationContext(recoverySettings), validateAllProperties: true);
        });

        services.Configure<RecoverySettings>(RecoverySettings.Names.TransientHttpFault, Configuration.GetSection($"{nameof(RecoverySettings)}:{RecoverySettings.Names.TransientHttpFault}"))
            .PostConfigure<RecoverySettings>(RecoverySettings.Names.TransientHttpFault, recoverySettings =>
        {
            Validator.ValidateObject(recoverySettings, new ValidationContext(recoverySettings), validateAllProperties: true);
        });

        string? userAgentString = Configuration["UserAgentString"];

        services.AddHttpClient<ICachingTokenManager, FileCachingTokenManager>((IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var oAuthSettings = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
            httpClient.BaseAddress = oAuthSettings.TokenEndpoint;
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd(MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgentString);
        }).AddPolicyHandler((serviceProvider, httpRequest) =>
        {
            var recoverySettings = serviceProvider.GetRequiredService<IOptionsSnapshot<RecoverySettings>>().Get(RecoverySettings.Names.TransientHttpFault);
            return HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(recoverySettings.RetryAttempts, attempt => recoverySettings.CalculateRetryDelay(attempt));
        });
        services.AddScoped<ITokenManager>(serviceProvider => serviceProvider.GetRequiredService<ICachingTokenManager>());

        services.AddMemoryCache();
        services.AddHttpClient<ApiClient>((IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
            httpClient.BaseAddress = apiSettings.BaseAddress;
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd(MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgentString);
        }).AddPolicyHandler((serviceProvider, httpRequest) =>
        {
            var recoverySettings = serviceProvider.GetRequiredService<IOptionsSnapshot<RecoverySettings>>().Get(RecoverySettings.Names.TransientHttpFault);
            return HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(recoverySettings.RetryAttempts, attempt => recoverySettings.CalculateRetryDelay(attempt));
        });
        services.AddScoped<IApiClient, CachingApiClient>();

        services.AddSingleton<IClock, SystemClock>();

        services.AddLogging(logging => logging.AddFile(Configuration.GetSection("Serilog:FileLogging")));
    }
}
