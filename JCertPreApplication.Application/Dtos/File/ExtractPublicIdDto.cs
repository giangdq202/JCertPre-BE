using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.File
{
    /// <summary>
    /// Request DTO for extracting public ID from a file URL
    /// </summary>
    public class ExtractPublicIdDto
    {
        /// <summary>
        /// The complete URL of the file to extract public ID from
        /// </summary>
        [Required(ErrorMessage = "File URL is required")]
        [Url(ErrorMessage = "Must be a valid URL")]
        public string FileUrl { get; set; } = string.Empty;
    }
}
