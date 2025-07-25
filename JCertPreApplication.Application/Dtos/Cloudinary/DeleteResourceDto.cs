using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Cloudinary
{
    /// <summary>
    /// Data transfer object for deleting a resource from Cloudinary.
    /// </summary>
    public class DeleteResourceDto
    {
        /// <summary>
        /// The public ID of the resource to delete.
        /// </summary>
        [Required(ErrorMessage = "Public ID is required")]
        [StringLength(255, ErrorMessage = "Public ID cannot exceed 255 characters")]
        public string PublicId { get; set; } = string.Empty;
    }
} 