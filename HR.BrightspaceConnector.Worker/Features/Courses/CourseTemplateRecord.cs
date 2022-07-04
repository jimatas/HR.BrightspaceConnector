using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Courses
{
    public class CourseTemplateRecord : RecordBase
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public IEnumerable<int> ParentOrgUnitIds { get; set; } = Enumerable.Empty<int>();
    }
}
