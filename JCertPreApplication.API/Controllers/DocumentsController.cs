using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Features.Documents;
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
        /// Uploads an image document.
        /// </summary>
        [HttpPost("upload/image")]
        public async Task<IActionResult> UploadImageDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadImageDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Uploads a video document.
        /// </summary>
        [HttpPost("upload/video")]
        public async Task<IActionResult> UploadVideoDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadVideoDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Uploads a raw document.
        /// </summary>
        [HttpPost("upload/document")]
        public async Task<IActionResult> UploadRawDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadRawDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Gets document by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            var result = await _documentService.GetDocumentByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets documents by lesson ID.
        /// </summary>
        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetDocumentsByLessonId(Guid lessonId)
        {
            var result = await _documentService.GetDocumentsByLessonIdAsync(lessonId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a document.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return Ok(new { message = "Document deleted successfully" });
        }
    }
}
