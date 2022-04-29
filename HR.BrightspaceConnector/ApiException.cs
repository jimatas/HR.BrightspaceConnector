using System.Net;

namespace HR.BrightspaceConnector
{
    public class ApiException : Exception
    {
        #region Constructors
        public ApiException() { }
        public ApiException(string? message) : base(message) { }
        public ApiException(string? message, Exception? innerException) : base(message, innerException) { }
        #endregion

        /// <summary>
        /// The HTTP status code that was returned in the response.
        /// </summary>
        public HttpStatusCode? StatusCode { get; init; }

        /// <summary>
        /// A problem details object that may have been provided by the origin server to convey further information about the error.
        /// </summary>
        public ProblemDetails? ProblemDetails { get; init; }
    }
}
