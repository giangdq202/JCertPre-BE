using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Text;

namespace JCertPreApplication.UnitTests.Common.Helpers;

public static class FormFileHelper
{
    public static Mock<IFormFile> CreateMockFile(
        string fileName = "test.jpg",
        string contentType = "image/jpeg",
        long length = 1024,
        byte[]? content = null)
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(length);
        
        content ??= Encoding.UTF8.GetBytes("test file content");
        var stream = new MemoryStream(content);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        return mockFile;
    }

    public static IFormFile CreateValidImageFile(long sizeInBytes = 1024)
    {
        return CreateMockFile("image.jpg", "image/jpeg", sizeInBytes).Object;
    }

    public static IFormFile CreateValidVideoFile(long sizeInBytes = 1024)
    {
        return CreateMockFile("video.mp4", "video/mp4", sizeInBytes).Object;
    }

    public static IFormFile CreateValidPdfFile(long sizeInBytes = 1024)
    {
        return CreateMockFile("document.pdf", "application/pdf", sizeInBytes).Object;
    }

    public static IFormFile CreateInvalidFile(long sizeInBytes = 1024)
    {
        return CreateMockFile("invalid.txt", "text/plain", sizeInBytes).Object;
    }

    public static IFormFile CreateOversizedImageFile()
    {
        // 51MB - exceeds 50MB limit for images
        return CreateMockFile("large.jpg", "image/jpeg", 51 * 1024 * 1024).Object;
    }

    public static IFormFile CreateOversizedVideoFile()
    {
        // 501MB - exceeds 500MB limit for videos
        return CreateMockFile("large.mp4", "video/mp4", 501L * 1024 * 1024).Object;
    }

    public static IFormFile CreateOversizedDocumentFile()
    {
        // 101MB - exceeds 100MB limit for documents
        return CreateMockFile("large.pdf", "application/pdf", 101 * 1024 * 1024).Object;
    }
}
