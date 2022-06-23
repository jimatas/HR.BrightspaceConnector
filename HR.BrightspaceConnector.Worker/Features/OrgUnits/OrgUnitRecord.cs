using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.OrgUnits
{
    public class OrgUnitRecord : RecordBase
    {
        public int? Type { get; set; }
        public string? TypeCode { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public IEnumerable<int> Parents { get; set; } = Enumerable.Empty<int>();
    }
}
