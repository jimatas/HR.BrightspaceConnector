namespace HR.BrightspaceConnector.Infrastructure
{
    public class PagingInfo
    {
        public string? Bookmark { get; set; }
        public bool HasMoreItems { get; set; } = false;
    }
}
