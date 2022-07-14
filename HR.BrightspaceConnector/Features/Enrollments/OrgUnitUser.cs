using HR.BrightspaceConnector.Features.Users;

namespace HR.BrightspaceConnector.Features.Enrollments
{
    public class OrgUnitUser
    {
        public User? User { get; set; }
        public RoleInfo? Role { get; set; }
    }
}
