using JCertPreApplication.Application.Dtos.File;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Contracts
{
    /// <summary>
    /// Generic file service interface that abstracts file storage operations.
    /// Supports multiple storage providers (Appwrite, AWS S3, Azure Blob, etc.) 
    /// without coupling to any specific implementation.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Uploads an image file to the configured storage provider.
        /// </summary>
        /// <param name="file">The file from the HTTP request.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Generic file upload result containing file URL and metadata.</returns>
        Task<FileUploadResult> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads a video or audio file to the configured storage provider.
        /// </summary>
        /// <param name="file">The file from the HTTP request.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Generic file upload result containing file URL and metadata.</returns>
        Task<FileUploadResult> UploadVideoAsync(IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads a document or raw file to the configured storage provider.
        /// </summary>
        /// <param name="file">The file from the HTTP request.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Generic file upload result containing file URL and metadata.</returns>
        Task<FileUploadResult> UploadDocumentAsync(IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a file from the storage provider using its public ID.
        /// </summary>
        /// <param name="publicId">The public ID/identifier of the file to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Result indicating success or failure of the deletion operation.</returns>
        Task<FileDeletionResult> DeleteFileAsync(string publicId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of files from the storage provider.
        /// </summary>
        /// <param name="maxResults">Maximum number of items per page (1-500).</param>
        /// <param name="nextCursor">Cursor from previous page; null for first page.</param>
        /// <param name="resourceType">Type of resource to filter: "image", "video", "audio", or "document".</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A page of file resources with cursor for next page (if available).</returns>
        Task<FileResourcesPageDto> GetResourcesPageAsync(int maxResults = 100, string? nextCursor = null, string resourceType = "image", CancellationToken cancellationToken = default);
    }
}
