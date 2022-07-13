namespace HR.BrightspaceConnector.Features.Enrollments
{
    public static class EnrollmentRecordExtensions
    {
        public static EnrollmentData ToEnrollmentData(this EnrollmentRecord enrollmentRecord)
        {
            return new EnrollmentData
            {
                IsCascading = null,
                OrgUnitId = enrollmentRecord.OrgUnitId,
                RoleId = enrollmentRecord.RoleId,
                UserId = enrollmentRecord.UserId
            };
        }

        public static CreateEnrollmentData ToCreateEnrollmentData(this EnrollmentRecord enrollmentRecord)
        {
            return new CreateEnrollmentData
            {
                OrgUnitId = enrollmentRecord.OrgUnitId,
                RoleId = enrollmentRecord.RoleId,
                UserId = enrollmentRecord.UserId
            };
        }
    }
}
