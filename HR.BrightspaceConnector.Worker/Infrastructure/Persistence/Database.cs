using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure.Hosting;
using HR.Common.Utilities;

using Microsoft.EntityFrameworkCore;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        private readonly BrightspaceDbContext dbContext;
        private readonly IHostEnvironment environment;
        private readonly ILogger logger;

        public Database(BrightspaceDbContext dbContext, IHostEnvironment environment, ILogger<Database> logger)
        {
            this.dbContext = dbContext;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = $"sync_out_brightspace_{environment.GetStoredProcedureEnvironmentName()}_user_GetNextEvents";
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var users = await dbContext.Users.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return users.SingleOrDefault();
        }

        public async Task<OrgUnitRecord?> GetNextCustomOrgUnitAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = $"sync_out_brightspace_{environment.GetStoredProcedureEnvironmentName()}_customOrgUnit_GetNextEvents";
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var customOrgUnits = await dbContext.OrgUnits.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return customOrgUnits.SingleOrDefault();
        }

        public async Task<OrgUnitRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = $"sync_out_brightspace_{environment.GetStoredProcedureEnvironmentName()}_department_GetNextEvents";
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var departments = await dbContext.OrgUnits.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return departments.SingleOrDefault();
        }

        public async Task<CourseTemplateRecord?> GetNextCourseTemplateAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = $"sync_out_brightspace_{environment.GetStoredProcedureEnvironmentName()}_courseTemplate_GetNextEvents";
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var courseTemplates = await dbContext.CourseTemplates.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return courseTemplates.SingleOrDefault();
        }

        public async Task<CourseOfferingRecord?> GetNextCourseOfferingAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = $"sync_out_brightspace_{environment.GetStoredProcedureEnvironmentName()}_courseOffering_GetNextEvents";
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var courseOfferings = await dbContext.CourseOfferings.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return courseOfferings.SingleOrDefault();
        }

        public async Task<EnrollmentRecord?> GetNextCourseOfferingEnrollmentAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = $"sync_out_brightspace_{environment.GetStoredProcedureEnvironmentName()}_courseOfferingEnrollment_GetNextEvents";
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var courseOfferingEnrollments = await dbContext.CourseOfferingEnrollments.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return courseOfferingEnrollments.SingleOrDefault();
        }

        public async Task MarkAsHandledAsync(
            int eventId,
            bool success,
            int? id,
            string? message,
            CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Executing stored procedure 'sync_event_MarkHandled'.");

            await dbContext.Database.ExecuteSqlInterpolatedAsync($"sync_event_MarkHandled {eventId}, {success}, {id?.ToString()}, {message}", cancellationToken).WithoutCapturingContext();
        }
    }
}
