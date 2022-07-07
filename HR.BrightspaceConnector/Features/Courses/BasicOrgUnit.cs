namespace HR.BrightspaceConnector.Features.Courses
{
    /// <summary>
    /// This composite contains basic information about an organizational unit to which a course offering is related.
    /// </summary>
    public class BasicOrgUnit
    {
        public int? Identifier { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
