using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Features.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages document operations including upload, retrieval, and deletion.
    /// </summary>
    [Route("api/documents")]
    [ApiController]
    [Tags("Documents")]
    [Produces("application/json")]
    [Authorize]
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
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> UploadImageDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadImageDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Uploads a video document.
        /// </summary>
        [HttpPost("upload/video")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> UploadVideoDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadVideoDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Uploads a raw document.
        /// </summary>
        [HttpPost("upload/document")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> UploadRawDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadRawDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Gets document by ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            var result = await _documentService.GetDocumentByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets documents by lesson ID.
        /// </summary>
        [HttpGet("lesson/{lessonId}")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetDocumentsByLessonId(Guid lessonId)
        {
            var result = await _documentService.GetDocumentsByLessonIdAsync(lessonId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a document.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return Ok(new { message = "Document deleted successfully" });
        }
    }
}
