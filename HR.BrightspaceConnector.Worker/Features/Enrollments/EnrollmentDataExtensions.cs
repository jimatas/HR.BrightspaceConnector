using HR.BrightspaceConnector.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments
{
    public static class EnrollmentDataExtensions
    {
        public static long? Identifier(this CreateEnrollmentData enrollmentData)
        {
            return enrollmentData.OrgUnitId is null || enrollmentData.UserId is null ? null
                : (long)ElegantPairingFunctions.Pair((uint)enrollmentData.OrgUnitId, (uint)enrollmentData.UserId);
        }
    }
}
