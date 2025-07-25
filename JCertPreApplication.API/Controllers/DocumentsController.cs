using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Features.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Upload a new image document
        /// </summary>
        /// <param name="createDocumentDto">The document data to upload</param>
        /// <returns>The uploaded document information</returns>
        [HttpPost("upload/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentDto>> UploadImageDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadImageDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Upload a new video document
        /// </summary>
        /// <param name="createDocumentDto">The document data to upload</param>
        /// <returns>The uploaded document information</returns>
        [HttpPost("upload/video")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentDto>> UploadVideoDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadVideoDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Upload a new raw document
        /// </summary>
        /// <param name="createDocumentDto">The document data to upload</param>
        /// <returns>The uploaded document information</returns>
        [HttpPost("upload/document")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentDto>> UploadRawDocument([FromForm] CreateDocumentDto createDocumentDto)
        {
            var result = await _documentService.UploadRawDocumentAsync(createDocumentDto);
            return Ok(result);
        }

        /// <summary>
        /// Get a document by its ID
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <returns>The document information</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentDto>> GetDocumentById(Guid id)
        {
            var result = await _documentService.GetDocumentByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Get all documents for a specific lesson
        /// </summary>
        /// <param name="lessonId">The lesson ID</param>
        /// <returns>List of documents for the lesson</returns>
        [HttpGet("lesson/{lessonId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<DocumentDto>>> GetDocumentsByLessonId(Guid lessonId)
        {
            var result = await _documentService.GetDocumentsByLessonIdAsync(lessonId);
            return Ok(result);
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="id">The document ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDocument(Guid id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return NoContent();
        }
    }
}
