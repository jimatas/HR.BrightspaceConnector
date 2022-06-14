using HR.BrightspaceConnector.Security;
using HR.BrightspaceConnector.Utilities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Tests
{
    public abstract class IntegrationTestsBase : IDisposable
    {
        private HttpClient? apiClientHttpClient;
        private HttpClient? tokenManagerHttpClient;

        static IntegrationTestsBase()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("C:\\Development\\HR.BrightspaceConnector\\HR.BrightspaceConnector.Worker\\appsettings.json")
                .AddUserSecrets("dotnet-HR.BrightspaceConnector.Worker-DF9AA187-1E6C-4D0A-BFF5-3C5F247DED74")
                .AddEnvironmentVariables()
                .Build();
        }

        protected static IConfiguration Configuration { get; }

        protected IApiClient CreateApiClient()
        {
            var apiSettings = Configuration.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
            var oAuthSettings = Configuration.GetRequiredSection(nameof(OAuthSettings)).Get<OAuthSettings>();
            var jsonOptions = CreateJsonOptions();

            apiClientHttpClient ??= CreateHttpClient(apiSettings.BaseAddress!);
            tokenManagerHttpClient ??= CreateHttpClient(oAuthSettings.TokenEndpoint!);

            return new ApiClient(apiClientHttpClient, Options.Create(jsonOptions), Options.Create(apiSettings),
                new FileCachingTokenManager(tokenManagerHttpClient, Options.Create(jsonOptions), Options.Create(oAuthSettings), new SystemClock()));

            static HttpClient CreateHttpClient(Uri baseAddress)
            {
                var httpClient = new HttpClient { BaseAddress = baseAddress };
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd(MediaTypeNames.Application.Json);
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Configuration["UserAgentString"]);

                return httpClient;
            }

            static JsonSerializerOptions CreateJsonOptions()
            {
                JsonSerializerOptions jsonOptions = new()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                jsonOptions.Converters.Add(new JsonStringEnumConverter());

                return jsonOptions;
            }
        }

        public void Dispose()
        {
            apiClientHttpClient?.Dispose();
            apiClientHttpClient = null;

            tokenManagerHttpClient?.Dispose();
            tokenManagerHttpClient = null;

            GC.SuppressFinalize(this);
        }
    }
}
