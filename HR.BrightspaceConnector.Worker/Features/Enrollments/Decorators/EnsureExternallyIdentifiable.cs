using HR.BrightspaceConnector.Features.Enrollments.Queries;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments.Decorators
{
    public class EnsureExternallyIdentifiable : IQueryHandlerWrapper<GetNextEnrollment, EnrollmentRecord?>
    {
        public async Task<EnrollmentRecord?> HandleAsync(GetNextEnrollment query, HandlerDelegate<EnrollmentRecord?> next, CancellationToken cancellationToken)
        {
            var enrollment = await next().WithoutCapturingContext();
            if (enrollment is not null)
            {
                if (enrollment.SyncExternalKey is null && enrollment.OrgUnitId is not null && enrollment.UserId is not null)
                {
                    enrollment.SyncExternalKey = ElegantPairingFunctions.Pair((uint)enrollment.OrgUnitId, (uint)enrollment.UserId).ToString();
                }
                else if ((enrollment.OrgUnitId is null || enrollment.UserId is null) && enrollment.SyncExternalKey is not null)
                {
                    ElegantPairingFunctions.Unpair(ulong.Parse(enrollment.SyncExternalKey), out uint orgUnitId, out uint userId);
                    enrollment.OrgUnitId = (int)orgUnitId;
                    enrollment.UserId = (int)userId;
                }
            }
            return enrollment;
        }
    }
}
