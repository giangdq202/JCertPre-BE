using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Features.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles document management, including upload for images, videos, and raw documents.
    /// </summary>
    [Route("api/documents")]
    [ApiController]
    [Tags("Documents")]
    [Produces("application/json")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        /// <summary>
        /// Uploads a new image document (JPG, PNG, JPEG only).
        /// </summary>
        /// <param name="createDocumentDto">The image document data to upload.</param>
        /// <returns>The uploaded document information.</returns>
        [HttpPost("upload/image")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadImageDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadImageDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Uploads a new video document (MP4 only).
        /// </summary>
        /// <param name="createDocumentDto">The video document data to upload.</param>
        /// <returns>The uploaded document information.</returns>
        [HttpPost("upload/video")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadVideoDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadVideoDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Uploads a new raw document (PDF, Word, Excel, PowerPoint only).
        /// </summary>
        /// <param name="createDocumentDto">The document data to upload.</param>
        /// <returns>The uploaded document information.</returns>
        [HttpPost("upload/document")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadRawDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadRawDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a document by its unique identifier.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The document information.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            var result = await _documentService.GetDocumentByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all documents associated with a specific lesson.
        /// </summary>
        /// <param name="lessonId">The lesson ID.</param>
        /// <returns>List of documents for the lesson.</returns>
        [HttpGet("lesson/{lessonId}")]
        [ProducesResponseType(typeof(ICollection<DocumentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDocumentsByLessonId(Guid lessonId)
        {
            var result = await _documentService.GetDocumentsByLessonIdAsync(lessonId);
            return Ok(result);
        }

        /// <summary>
        /// Permanently deletes a document from both storage and database.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>A confirmation message.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return Ok(new { message = "Document deleted successfully" });
        }
    }
}
