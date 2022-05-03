using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector.Utilities
{
    public static class ApiExceptionExtensions
    {
        /// <summary>
        /// Returns an error message that contains the information available in the <see cref="ApiException.ProblemDetails"/> object, if one is set. Otherwise simply returns the exception message.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>A possibly more detailed error message.</returns>
        public static string GetErrorMessage(this ApiException exception)
        {
            return exception.ProblemDetails is ProblemDetails error ? $"{error.Title}: {error.Detail}" : exception.Message;
        }
    }
}
