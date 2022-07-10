using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Courses
{
    public static class CourseOfferingRecordExtensions
    {
        public static CreateCourseOffering ToCreateCourseOffering(this CourseOfferingRecord courseOfferingRecord)
        {
            return new CreateCourseOffering
            {
                CanSelfRegister = courseOfferingRecord.CanSelfRegister,
                Code = courseOfferingRecord.Code,
                CourseTemplateId = courseOfferingRecord.CourseTemplateId,
                Description = !string.IsNullOrEmpty(courseOfferingRecord.Description)
                    ? new RichTextInput { Content = courseOfferingRecord.Description, Type = TextContentType.Html }
                    : null,
                EndDate = courseOfferingRecord.EndDate,
                ForceLocale = courseOfferingRecord.ForceLocale,
                IsActive = courseOfferingRecord.IsActive,
                LocaleId = courseOfferingRecord.LocaleId,
                Name = courseOfferingRecord.Name,
                Path = courseOfferingRecord.Path,
                SemesterId = courseOfferingRecord.SemesterId,
                ShowAddressBook = courseOfferingRecord.ShowAddressBook,
                StartDate = courseOfferingRecord.StartDate
            };
        }
    }
}
