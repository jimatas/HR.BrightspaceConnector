namespace HR.BrightspaceConnector.Infrastructure
{
    /// <summary>
    /// In certain error scenarios the Brightspace APIs return a custom JSON payload instead of a <see cref="ProblemDetails"/> block to further describe the error that occurred.
    /// </summary>
    internal class ErrorResponse
    {
        public IEnumerable<ErrorDetail> Errors { get; set; } = Enumerable.Empty<ErrorDetail>();

        public class ErrorDetail
        {
            public string? Message { get; set; }
        }
    }
}
