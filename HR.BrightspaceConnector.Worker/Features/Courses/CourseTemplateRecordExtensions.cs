namespace HR.BrightspaceConnector.Features.Courses
{
    public static class CourseTemplateRecordExtensions
    {
        public static CreateCourseTemplate ToCreateCourseTemplate(this CourseTemplateRecord courseTemplateRecord)
        {
            return new CreateCourseTemplate
            {
                Code = courseTemplateRecord.Code,
                Name = courseTemplateRecord.Name,
                ParentOrgUnitIds = courseTemplateRecord.ParentOrgUnitIds,
                Path = courseTemplateRecord.Path
            };
        }

        public static CourseTemplateInfo ToCourseTemplateInfo(this CourseTemplateRecord courseTemplateRecord)
        {
            return new CourseTemplateInfo
            {
                Code = courseTemplateRecord.Code,
                Name = courseTemplateRecord.Name
            };
        }
    }
}
