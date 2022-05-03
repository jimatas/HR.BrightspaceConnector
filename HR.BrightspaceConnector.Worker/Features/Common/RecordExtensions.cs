namespace HR.BrightspaceConnector.Features.Common
{
    public static class RecordExtensions
    {
        public static bool IsToBeCreated(this RecordBase record) => new[] { 'c', 'C' }.Any(c => record.SyncAction == c);
        public static bool IsToBeUpdated(this RecordBase record) => new[] { 'u', 'U' }.Any(c => record.SyncAction == c);
        public static bool IsToBeDeleted(this RecordBase record) => new[] { 'd', 'D' }.Any(c => record.SyncAction == c);
    }
}
