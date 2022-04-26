using HR.BrightspaceConnector;
using HR.BrightspaceConnector.Security;

using Microsoft.Extensions.Options;

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

        services.Configure<JsonSerializerOptions>(jsonOptions =>
        {
            jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            jsonOptions.AllowTrailingCommas = true;
            jsonOptions.PropertyNameCaseInsensitive = true; // Web default
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Web default
            jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString; // Web default
        });

        services.Configure<OAuthSettings>(Configuration.GetSection(nameof(OAuthSettings)));

        services.AddHttpClient(OAuthSettings.HttpClientName, (IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var oAuthSettings = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
            httpClient.BaseAddress = new Uri(oAuthSettings.TokenEndpointUrl!.TrimEnd('/'));
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd(MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("HR.BrightspaceConnector.Client/1.0");
        });

        services.AddScoped<ICachingTokenManager, CachingTokenManager>();
        services.AddScoped<ITokenManager>(serviceProvider => serviceProvider.GetRequiredService<ICachingTokenManager>());

        services.AddSingleton<IClock, SystemClock>();
    }
}
