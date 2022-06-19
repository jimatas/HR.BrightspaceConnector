using System.Data.Common;

namespace HR.BrightspaceConnector.Utilities
{
    public static class ExceptionExtensions
    {
        public static bool IsTimeoutException(this Exception exception)
        {
            return (exception is DbException && exception.Message.Contains("Timeout Expired", StringComparison.OrdinalIgnoreCase))
                || (exception.InnerException is not null && exception.InnerException.IsTimeoutException());
        }
    }
}
