namespace JCertPreApplication.Application.Dtos.User
{
    /// <summary>
    /// 🎭 Role information data transfer object.
    /// </summary>
    /// <remarks>
    /// This DTO contains role information used for user creation and role management.
    /// </remarks>
    public class RoleDto
    {
        /// <summary>
        /// Unique role identifier.
        /// </summary>
        /// <example>8dd36044-84d4-4e4b-8162-34b7a421657c</example>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Role name (used for role assignment).
        /// </summary>
        /// <example>STUDENT</example>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Role description.
        /// </summary>
        /// <example>Student role</example>
        public string? Description { get; set; }
    }
}
