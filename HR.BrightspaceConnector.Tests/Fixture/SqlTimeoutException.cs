using System.Data.Common;

namespace HR.BrightspaceConnector.Tests.Fixture
{
    public class SqlTimeoutException : DbException
    {
        public const string DefaultMessage = "Execution Timeout Expired. The timeout period elapsed prior to completion of the operation or the server is not responding.";

        public SqlTimeoutException() : this(DefaultMessage) { }
        public SqlTimeoutException(string customMessage) : base(customMessage) { }
    }
}
