﻿using HR.BrightspaceConnector;
using HR.BrightspaceConnector.Security;
using HR.BrightspaceConnector.Utilities;

using Microsoft.Extensions.Options;

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

        services.Configure<ApiSettings>(Configuration.GetSection(nameof(ApiSettings))).PostConfigure<ApiSettings>(apiSettings =>
        {
            Validator.ValidateObject(apiSettings, new ValidationContext(apiSettings), validateAllProperties: true);
        });

        services.Configure<OAuthSettings>(Configuration.GetSection(nameof(OAuthSettings))).PostConfigure<OAuthSettings>(oAuthSettings =>
        {
            Validator.ValidateObject(oAuthSettings, new ValidationContext(oAuthSettings), validateAllProperties: true);
        });

        string? userAgentString = Configuration["UserAgentString"];

        services.AddHttpClient<ICachingTokenManager, FileCachingTokenManager>((IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var oAuthSettings = serviceProvider.GetRequiredService<IOptions<OAuthSettings>>().Value;
            httpClient.BaseAddress = oAuthSettings.TokenEndpoint;
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd(MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgentString);
        });

        services.AddScoped<ITokenManager>(serviceProvider => serviceProvider.GetRequiredService<ICachingTokenManager>());

        services.AddHttpClient<IApiClient, ApiClient>((IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
            httpClient.BaseAddress = apiSettings.BaseAddress;
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd(MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgentString);
        });

        services.AddSingleton<IClock, SystemClock>();
    }
}
