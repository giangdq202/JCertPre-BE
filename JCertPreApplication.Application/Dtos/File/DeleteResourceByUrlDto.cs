using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.File
{
    /// <summary>
    /// Request DTO for deleting a file by its URL
    /// </summary>
    public class DeleteResourceByUrlDto
    {
        /// <summary>
        /// The complete URL of the file to delete
        /// </summary>
        [Required(ErrorMessage = "File URL is required")]
        [Url(ErrorMessage = "Must be a valid URL")]
        public string FileUrl { get; set; } = string.Empty;
    }
}
