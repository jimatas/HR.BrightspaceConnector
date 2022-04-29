using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector
{
    public class ApiSettings : IValidatableObject
    {
        /// <summary>
        /// URI containing the base address of the API up to, but excluding, the product component and version number. Note that it should end with a trailing slash.
        /// For instance, https://brightspace.hr.nl/d2l/api/
        /// </summary>
        [Required]
        public Uri? BaseAddress { get; set; }

        /// <summary>
        /// The requisite version of the Learning Platform (LP) component of the LMS.
        /// For instance, 1.36
        /// </summary>
        [Required]
        public Version? LearningPlatformVersion { get; set; }

        /// <summary>
        /// The requisite version of the Learning Environment (LE) component of the LMS.
        /// For instance, 1.61
        /// </summary>
        [Required]
        public Version? LearningEnvironmentVersion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BaseAddress?.AbsolutePath.EndsWith('/') == false)
            {
                yield return new ValidationResult($"The {nameof(BaseAddress)} field must end with a trailing slash.", new[] { nameof(BaseAddress) });
            }
        }
    }
}
