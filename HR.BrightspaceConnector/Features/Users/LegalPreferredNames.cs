namespace HR.BrightspaceConnector.Features.Users
{
    public class LegalPreferredNames
    {
        /// <summary>
        /// Users must always have legal names; when setting names, you must always provide a value for the two Legal name fields.
        /// These fields both must be valid name strings for users: they cannot be solely composed of whitespace.
        /// </summary>
        public string? LegalFirstName { get; set; }

        /// <summary>
        /// Users must always have legal names; when setting names, you must always provide a value for the two Legal name fields.
        /// These fields both must be valid name strings for users: they cannot be solely composed of whitespace.
        /// </summary>
        public string? LegalLastName { get; set; }

        /// <summary>
        /// Users need not have preferred names; when setting names, you may provide null for one or both of the Preferred name fields to note that the user has no preferred version of that name other than their legal name(s).
        /// If not null, these fields both must be valid name strings for users: they cannot be solely composed of whitespace.
        /// </summary>
        public string? PreferredFirstName { get; set; }

        /// <summary>
        /// Users need not have preferred names; when setting names, you may provide null for one or both of the Preferred name fields to note that the user has no preferred version of that name other than their legal name(s).
        /// If not null, these fields both must be valid name strings for users: they cannot be solely composed of whitespace.
        /// </summary>
        public string? PreferredLastName { get; set; }

        /// <summary>
        /// This field provides a way to set the last name by which the back end service will sort users for presentation in various lists.
        /// Added with LP API v1.34
        /// </summary>
        public string? SortLastName { get; set; }
    }
}
