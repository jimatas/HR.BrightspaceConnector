namespace HR.BrightspaceConnector.Features.Roles
{
    public class Role
    {
        public string? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public string? Code { get; set; }

        #region Available in LP's unstable contract
        public string? Description { get; set; }
        public string? RoleAlias { get; set; }
        public bool? IsCascading { get; set; }
        public bool? AccessFutureCourses { get; set; }
        public bool? AccessInactiveCourses { get; set; }
        public bool? AccessPastCourses { get; set; }
        public bool? ShowInGrades { get; set; }
        public bool? ShowInUserProgress { get; set; }
        public bool? InClassList { get; set; }
        #endregion
    }
}
