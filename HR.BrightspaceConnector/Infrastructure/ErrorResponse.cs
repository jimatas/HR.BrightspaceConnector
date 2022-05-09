namespace HR.BrightspaceConnector.Infrastructure
{
    internal class ErrorResponse
    {
        public IEnumerable<ErrorDetail> Errors { get; set; } = Enumerable.Empty<ErrorDetail>();

        public class ErrorDetail
        {
            public string? Message { get; set; }
        }
    }
}
