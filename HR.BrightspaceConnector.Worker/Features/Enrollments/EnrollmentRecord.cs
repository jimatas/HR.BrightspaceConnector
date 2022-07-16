using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments
{
    public class EnrollmentRecord : RecordBase
    {
        public int? OrgUnitId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public new string? SyncExternalKey
        {
            get => OrgUnitId is null || UserId is null ? null : ElegantPairingFunctions.Pair((uint)OrgUnitId, (uint)UserId).ToString();
            set
            {
                if (value is not null && ulong.TryParse(value, out var identifier))
                {
                    ElegantPairingFunctions.Unpair(identifier, out uint orgUnitId, out uint userId);
                    OrgUnitId = (int)orgUnitId;
                    UserId = (int)userId;
                }
            }
        }
    }
}
