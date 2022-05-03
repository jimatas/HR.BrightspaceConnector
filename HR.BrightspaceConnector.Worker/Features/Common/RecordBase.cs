namespace HR.BrightspaceConnector.Features.Common
{
    /// <summary>
    /// Base class for types that represent a stored procedure result.
    /// </summary>
    public abstract class RecordBase
    {
        public int? SyncEventId { get; set; }
        public char? SyncAction { get; set; }
        public string? SyncExternalKey { get; set; }
        public string? SyncInternalKey { get; set; }
    }
}
