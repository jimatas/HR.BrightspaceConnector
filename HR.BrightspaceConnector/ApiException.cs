using System.Net;

namespace HR.BrightspaceConnector
{
    /// <summary>
    /// Represents an error that was returned by the server in response to an unsuccessful API request. 
    /// Any further information about the error that was provided by the server will be available through the <see cref="ProblemDetails"/> property.
    /// </summary>
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
