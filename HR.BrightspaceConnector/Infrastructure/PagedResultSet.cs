namespace HR.BrightspaceConnector.Infrastructure
{
    /// <summary>
    /// Some actions can prompt the service to return very large sets of results. In these cases, the service may return data to callers in segments of the entire set. 
    /// You can use these segments to partition your retrieval of the entire data set (for example, as a background synchronization process), or to create an interactive way for users to page through the data set until they find a particular item they're looking for.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResultSet<T>
    {
        public PagingInfo PagingInfo { get; set; } = new();
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
