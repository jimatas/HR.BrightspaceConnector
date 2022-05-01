namespace HR.BrightspaceConnector.Utilities
{
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Returns a short informational message consisting of the HTTP status code and description of the response.
        /// For instance, "HTTP 404 - Not Found"
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public static string GetStatusMessage(this HttpResponseMessage httpResponse)
        {
            return $"HTTP {(int)httpResponse.StatusCode} - {httpResponse.ReasonPhrase}";
        }
    }
}
