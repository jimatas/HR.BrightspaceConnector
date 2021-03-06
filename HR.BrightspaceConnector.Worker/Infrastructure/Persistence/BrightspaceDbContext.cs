using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Sections;
using HR.BrightspaceConnector.Features.Users;

using Microsoft.EntityFrameworkCore;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    public class BrightspaceDbContext : DbContext
    {
        public BrightspaceDbContext() { }
        public BrightspaceDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserRecord> Users => Set<UserRecord>();
        public DbSet<OrgUnitRecord> OrgUnits => Set<OrgUnitRecord>();
        public DbSet<CourseTemplateRecord> CourseTemplates => Set<CourseTemplateRecord>();
        public DbSet<CourseOfferingRecord> CourseOfferings => Set<CourseOfferingRecord>();
        public DbSet<EnrollmentRecord> CourseOfferingEnrollments => Set<EnrollmentRecord>();
        public DbSet<SectionRecord> CourseOfferingSections => Set<SectionRecord>();
    }
}
