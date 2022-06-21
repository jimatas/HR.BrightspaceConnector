using System;

namespace HR.BrightspaceConnector.Tests.Fixture
{
    public class NonRecoverableErrorException : Exception
    {
        public const string DefaultMessage = "A non-recoverable error occurred. The operation will not be retried.";

        public NonRecoverableErrorException() : this(DefaultMessage) { }
        public NonRecoverableErrorException(string customMessage) : base(customMessage) { }
    }
}
