using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Enrollments
{
    public class EnrollmentRecord : RecordBase
    {
        public int? OrgUnitId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}
