using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Courses
{
    public class CourseOfferingRecord : RecordBase
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? Path { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int? CourseTemplateId { get; set; }
        public int? SemesterId { get; set; }
        public int? LocaleId { get; set; }
        public bool? ForceLocale { get; set; }
        public bool? ShowAddressBook { get; set; }
        public string? Description { get; set; }
        public bool? CanSelfRegister { get; set; }
    }
}
