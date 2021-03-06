using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Sections;
using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector
{
    public interface IApiClient
    {
        #region Users
        /// <summary>
        /// Retrieve a list of all known user roles.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>role:detail:read</c>
        /// </remarks>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve data for one or more users.
        /// </summary>
        /// <remarks>
        /// You can use this action in a number of different ways, depending upon the query parameters you provide. 
        /// If you provide more than one of these parameters, this action selects among them in this order, regardless of the order you provide them in your URL:
        /// <list type="number">
        ///   <item>orgDefinedId. Find all users that have this org-defined ID string. Fetch the results in a JSON array.</item>
        ///   <item>userName. Find the single user that has this user name.</item>
        ///   <item>externalEmail. Find all users that exactly match this external email address string. Fetch the results in a JSON array.</item>
        ///   <item>bookmark. Use a paged result set to return the results. Fetch the segment of results immediately following your bookmark.</item>
        /// </list>
        /// If you provide none of these query parameters, this action behaves as if you'd passed an empty value for the bookmark parameter: it fetches the first segment of results.
        /// <para>
        /// Oauth2 Scopes: <c>users:userdata:read</c>
        /// </para>
        /// </remarks>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A (possibly empty) <see cref="IEnumerable{UserData}"/> over the results. For the default and 'bookmark' cases, this IEnumerable will be of type <see cref="PagedResultSet{UserData}"/>.</returns>
        Task<IEnumerable<UserData>> GetUsersAsync(UserQueryParameters? queryParameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new user entity.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>users:userdata:create</c>
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a UserData JSON block for the newly created user, to give you immediate access to the user's UserId property.</returns>
        Task<UserData> CreateUserAsync(CreateUserData user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update data for a particular user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a UserData JSON block for the user's updated data.</returns>
        Task<UserData> UpdateUserAsync(int userId, UpdateUserData user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a particular user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve legal, preferred, and sort names for a particular user.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>users:userdata:read</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LegalPreferredNames> GetLegalPreferredNamesAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update legal, preferred, and sort name data for a particular user.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>users:userdata:update</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="userNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a LegalPreferredNames JSON block for the user's updated name data.</returns>
        Task<LegalPreferredNames> UpdateLegalPreferredNamesAsync(int userId, LegalPreferredNames userNames, CancellationToken cancellationToken = default);
        #endregion

        #region OrgUnits
        /// <summary>
        /// Retrieve the organization properties information.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a Organization JSON data block containing the identifier, name, and time zone of the organization.</returns>
        Task<Organization> GetOrganizationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve all the known and visible org unit types.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<OrgUnitType>> GetOrgUnitTypesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve properties for all org units.
        /// </summary>
        /// <remarks>
        /// You can use the orgUnitType, orgUnitCode, and orgUnitName query parameters as filters to further narrow the list of org units this action retrieves. 
        /// Note that for orgUnitType, the back-end service expects to receive a valid org unit type ID value. 
        /// Note that orgUnitCode and orgUnitName both will search for matches that contain your parameter value.
        /// <para>
        /// You can use the exactOrgUnitCode and exactOrgUnitName query parameters as more precise filters for narrowing down the list of org units.
        /// If you already know the exact code or name of the org unit in question, we recommend you use these filters instead of the more general ones.
        /// </para>
        /// <para>
        /// Oauth2 Scopes: <c>organizations:organization:read</c>
        /// </para>
        /// </remarks>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PagedResultSet<OrgUnitProperties>> GetOrgUnitsAsync(OrgUnitQueryParameters? queryParameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a list of child-units for a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>organizations:organization:read</c>
        /// </remarks>
        /// <param name="parentOrgUnitId">The ID of the OrgUnit to retrieve the immediate children of.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a paged result set containing the resulting org unit blocks for the segment following your bookmark parameter (or the first segment if that parameter is empty or missing).</returns>
        Task<PagedResultSet<OrgUnit>> GetChildOrgUnitsAsync(int parentOrgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a list of descendent-units for a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>organizations:organization:read</c>
        /// </remarks>
        /// <param name="ancestorOrgUnitId">The ID of the OrgUnit to retrieve the descendants of.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a paged result set containing the resulting org unit blocks for the segment following your bookmark parameter (or the first segment if that parameter is empty or missing).</returns>
        Task<PagedResultSet<OrgUnit>> GetDescendantOrgUnitsAsync(int ancestorOrgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new custom org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>organizations:organization:create</c>
        /// </remarks>
        /// <param name="orgUnit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns an OrgUnit JSON data block containing the properties for the newly created org unit.</returns>
        Task<OrgUnit> CreateOrgUnitAsync(OrgUnitCreateData orgUnit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a custom org unit's properties.
        /// </summary>
        /// <remarks>
        /// Note that you can only update the Name, Code, and Path properties; the values of the other proprties in the OrgUnitProperties you provide are not used to update your specified org unit.
        /// <para>
        /// Oauth2 Scopes: <c>organizations:organization:update</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="orgUnit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns an OrgUnitProperties JSON data block containing the properties for the updated org unit.</returns>
        Task<OrgUnitProperties> UpdateOrgUnitAsync(int orgUnitId, OrgUnitProperties orgUnit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send an org unit to the recycle bin, with an option to immediately flush it from there as well, permanently deleting it.
        /// </summary>
        /// <remarks>
        /// Using this action has the same effect as deleting the org unit via the OrgUnit Editor (and placing it in the recycling bin) in the Brightspace UI.
        /// <para>
        /// Oauth2 Scopes: <c>organizations:organization:create</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="permanently">Permanently delete the org unit?</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteOrgUnitAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default);
        #endregion

        #region Courses
        /// <summary>
        /// Retrieve a course template.
        /// </summary>
        /// <param name="orgUnitId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a CourseTemplate JSON block.</returns>
        Task<CourseTemplate> GetCourseTemplateAsync(int orgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new course template.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>orgunits:coursetemplate:create</c>
        /// </remarks>
        /// <param name="courseTemplate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a CourseTemplate JSON block containing the data for the new course template.</returns>
        Task<CourseTemplate> CreateCourseTemplateAsync(CreateCourseTemplate courseTemplate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the information for a course template.
        /// </summary>
        /// <remarks>
        /// This action uses the data in your provided block to completely replace the associated course template's data on the service.
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="courseTemplate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateCourseTemplateAsync(int orgUnitId, CourseTemplateInfo courseTemplate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a course template.
        /// </summary>
        /// <remarks>
        /// Using this action is equivalent to using the route to send an org unit to the recycle bin. 
        /// You can restore the course template from the recycle bin, if needed.
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="permanently">Permanently delete the course template?</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteCourseTemplateAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a course offering.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>orgunits:course:read</c>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a CourseOffering JSON block with the provided course's information.</returns>
        Task<CourseOffering> GetCourseOfferingAsync(int orgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new course offering.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>orgunits:course:create</c>
        /// </remarks>
        /// <param name="courseOffering">New course offering properties.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a CourseOffering JSON block for the newly created course.</returns>
        Task<CourseOffering> CreateCourseOfferingAsync(CreateCourseOffering courseOffering, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a current course offering.
        /// </summary>
        /// <remarks>
        /// This action replaces the associated course offering's data with all the properties you provide.
        /// <para>
        /// Oauth2 Scopes: <c>orgunits:course:update</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="courseOffering">Updated course offering properties.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateCourseOfferingAsync(int orgUnitId, CourseOfferingInfo courseOffering, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a course offering.
        /// </summary>
        /// <remarks>
        /// Using this action is equivalent to using the route to send an org unit to the recycle bin. 
        /// You can restore the course offering from the recycle bin, if needed.
        /// <para>
        /// Oauth2 Scopes: <c>orgunits:course:delete</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="permanently">Permanently delete the course offering?</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteCourseOfferingAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default);
        #endregion

        #region Enrollments
        /// <summary>
        /// Retrieve enrollment details in an org unit for the provided user.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>enrollment:orgunit:read</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="orgUnitId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns an EnrollmentData JSON block.</returns>
        Task<EnrollmentData> GetEnrollmentAsync(int userId, int orgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve the collection of users enrolled in the identified org unit.
        /// </summary>
        /// <remarks>
        /// You can use a bookmark query parameter as a paging offset, to indicate that the service should return the segment of results immediately following your bookmark.
        /// <para>
        /// Oauth2 Scopes: <c>enrollment:orgunit:read</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> This action returns a paged result set containing the resulting OrgUnitUser data blocks for the segment following your bookmark parameter (or the first segment if the parameter is empty or missing).</returns>
        Task<PagedResultSet<OrgUnitUser>> GetEnrolledUsersAsync(int orgUnitId, EnrolledUserQueryParameters? queryParameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create or update a new enrollment for a user.
        /// </summary>
        /// <remarks>
        /// If the user doesn't already have an enrollment in the applicable org unit, this action creates a new enrollment; 
        /// if the user does already have an enrollment in the org unit, this action updates the enrollment in place to use the new role.
        /// <para>
        /// Oauth2 Scopes: <c>enrollment:orgunit:create</c>
        /// </para>
        /// </remarks>
        /// <param name="enrollment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns an EnrollmentData JSON block for the newly enrolled user.</returns>
        Task<EnrollmentData> CreateOrUpdateEnrollmentAsync(CreateEnrollmentData enrollment, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a user's enrollment in a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>enrollment:orgunit:delete</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="orgUnitId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Unlike most delete actions, this action returns an EnrollmentData JSON block showing the enrollment status just before this action deleted the user's enrollment.</returns>
        Task<EnrollmentData> DeleteEnrollmentAsync(int userId, int orgUnitId, CancellationToken cancellationToken = default);
        #endregion

        #region Sections
        /// <summary>
        /// Retrieve the settings for all sections in an org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>sections:section:read</c>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a SectionSettingsData JSON block in the Fetch form.</returns>
        Task<SectionSettingsData> GetSectionSettingsAsync(int orgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initialize the settings for all sections in a particular org unit.
        /// </summary>
        /// <remarks>
        /// You can only use this action to initialize the settings for all sections in an org unit. Once you've used this action to do so, you must subsequently use
        /// <list type="bullet">
        ///   <item><c>PUT /d2l/api/lp/(version)/(orgUnitId)/sections/settings</c> to manage the section settings for the org unit</item>
        ///   <item><c>POST /d2l/api/lp/(version)/(orgUnitId)/sections/ to create</c> sections as descendants of the org unit</item>
        /// </list>
        /// <para>
        /// Oauth2 Scopes: <c>sections:section:create</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="sectionSettings">New settings for sections in this org unit.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns the SectionSettingsData JSON block, in its Fetch form, for the org unit's new section properties.</returns>
        Task<SectionSettingsData> CreateSectionSettingsAsync(int orgUnitId, CreateSectionSettingsData sectionSettings, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the section settings for an org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>sections:section:update</c>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="sectionSettings">Updated settings for all sections in this org unit.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns the SectionSettingsData JSON block, in its Fetch form, for the org unit's updated section properties.</returns>
        Task<SectionSettingsData> UpdateSectionSettingsAsync(int orgUnitId, UpdateSectionSettingsData sectionSettings, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve all the sections for a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>sections:section:read</c>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a JSON array of SectionData blocks in the Fetch form.</returns>
        Task<IEnumerable<SectionData>> GetSectionsAsync(int orgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new section in a particular org unit.
        /// </summary>
        /// <remarks>
        /// You can only use this action to add a section to an org unit that's already been initialized with section settings for the org unit.
        /// <para>
        /// Oauth2 Scopes: <c>sections:section:create</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="section">New section data.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns the SectionData JSON block, in its Fetch form, for the org unit's new section.</returns>
        Task<SectionData> CreateSectionAsync(int orgUnitId, CreateOrUpdateSectionData section, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enroll a user in a section for a particular org unit.
        /// </summary>
        /// <remarks>
        /// Note that the user provided as input to this action must already be explicitly enrolled into the org unit identified by the action's orgUnitId route parameter.
        /// <para>
        /// Oauth2 Scopes: <c>sections:enrollment:create</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="sectionId"></param>
        /// <param name="enrollment">Enrollment block for enrollment in the section.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateSectionEnrollmentAsync(int orgUnitId, int sectionId, SectionEnrollment enrollment, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update information for a section in a particular org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>sections:section:update</c>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="sectionId"></param>
        /// <param name="section">Updated section data.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns the updated SectionData JSON block, in its Fetch form.</returns>
        Task<SectionData> UpdateSectionAsync(int orgUnitId, int sectionId, CreateOrUpdateSectionData section, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a section from a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>sections:section:delete</c>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="sectionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteSectionAsync(int orgUnitId, int sectionId, CancellationToken cancellationToken = default);
        #endregion
    }
}
