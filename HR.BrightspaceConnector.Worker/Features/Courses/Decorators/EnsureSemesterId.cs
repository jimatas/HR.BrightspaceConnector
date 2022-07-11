using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses.Queries;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector.Features.Courses.Decorators
{
    public class EnsureSemesterId : IQueryHandlerWrapper<GetNextCourseOffering, CourseOfferingRecord?>
    {
        private readonly IApiClient apiClient;
        private readonly StandardOrgUnitTypeCodes standardOrgUnitTypeCodes;

        public EnsureSemesterId(IApiClient apiClient, IOptions<StandardOrgUnitTypeCodes> standardOrgUnitTypeCodes)
        {
            this.apiClient = apiClient;
            this.standardOrgUnitTypeCodes = standardOrgUnitTypeCodes.Value;
        }

        public async Task<CourseOfferingRecord?> HandleAsync(GetNextCourseOffering query, HandlerDelegate<CourseOfferingRecord?> next, CancellationToken cancellationToken)
        {
            CourseOfferingRecord? retrievedCourseOffering = await next().WithoutCapturingContext();
            if (retrievedCourseOffering is not null && retrievedCourseOffering.SemesterId.IsNullOrDefault()
                && !string.IsNullOrEmpty(retrievedCourseOffering.SemesterCode))
            {
                var allOrgUnitTypes = await apiClient.GetOrgUnitTypesAsync(cancellationToken).WithoutCapturingContext();
                var semesterOrgUnitType = allOrgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, standardOrgUnitTypeCodes.Semester, StringComparison.OrdinalIgnoreCase));
                if (semesterOrgUnitType is not null)
                {
                    var queryParams = new OrgUnitQueryParameters
                    {
                        ExactOrgUnitCode = retrievedCourseOffering.SemesterCode,
                        OrgUnitType = semesterOrgUnitType.Id
                    };

                    var matchingOrgUnits = await apiClient.GetOrgUnitsAsync(queryParams, cancellationToken).WithoutCapturingContext();
                    var semester = matchingOrgUnits.SingleOrDefault();
                    if (semester is not null)
                    {
                        retrievedCourseOffering.SemesterId = semester.Identifier;
                    }
                }
            }
            return retrievedCourseOffering;
        }
    }
}
