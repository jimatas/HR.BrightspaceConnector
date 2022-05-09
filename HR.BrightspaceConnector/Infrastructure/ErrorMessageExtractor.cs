using HR.BrightspaceConnector.Utilities;
using HR.Common.Utilities;

using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HR.BrightspaceConnector.Infrastructure
{
    internal static class ErrorMessageExtractor
    {
        public static async Task<AsyncOutResult<bool, string>> TryGetErrorMessageAsync(
            this HttpResponseMessage httpResponse,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default)
        {
            if (httpResponse.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
            {
                var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>(jsonOptions, cancellationToken).WithoutCapturingContext();
                if (errorResponse!.Errors.Any())
                {
                    var message = string.Join(Environment.NewLine,
                        errorResponse.Errors.Select(e => e.Message));

                    return (true, message);
                }
            }

            if (httpResponse.Content.Headers.ContentType?.MediaType == MediaTypeNames.Text.Html)
            {
                var contentBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken).WithoutCapturingContext();
                if (contentBody.Contains("Errors:"))
                {
                    const string pattern = @"Message:\s?""([^""]+)""";
                    var message = string.Join(Environment.NewLine,
                        Regex.Matches(contentBody, pattern).Select(match => match.Groups[1].Value));

                    return (true, message);
                }
            }

            return (false, string.Empty);
        }
    }
}
