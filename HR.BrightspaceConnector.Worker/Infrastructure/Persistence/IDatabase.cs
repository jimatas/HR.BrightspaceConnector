using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Users;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    /// <summary>
    /// Abstracts the source database and the stored procedures through which it is accessed.
    /// </summary>
    public interface IDatabase
    {
        Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default);
        Task<OrgUnitRecord?> GetNextCustomOrgUnitAsync(CancellationToken cancellationToken = default);
        Task<OrgUnitRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default);
        Task<CourseTemplateRecord?> GetNextCourseTemplateAsync(CancellationToken cancellationToken = default);
        Task<CourseOfferingRecord?> GetNextCourseOfferingAsync(CancellationToken cancellationToken = default);
        Task<EnrollmentRecord?> GetNextCourseOfferingEnrollmentAsync(CancellationToken cancellationToken = default);

        Task MarkAsHandledAsync(
            int eventId,
            bool success,
            string? externalKey,
            string? message,
            CancellationToken cancellationToken = default);
    }
}
