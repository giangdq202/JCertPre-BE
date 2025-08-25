using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Documents;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Helpers;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;

namespace JCertPreApplication.UnitTests.Features.Documents;

public class DocumentServiceTests
{
    private readonly Mock<IDocumentRepository> _mockDocumentRepository;
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly DocumentService _documentService;

    public DocumentServiceTests()
    {
        _mockDocumentRepository = new Mock<IDocumentRepository>();
        _mockLessonRepository = new Mock<ILessonRepository>();
        _mockFileService = new Mock<IFileService>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _documentService = new DocumentService(
            _mockDocumentRepository.Object,
            _mockLessonRepository.Object,
            _mockFileService.Object,
            _mockMapper.Object);
    }

    #region UploadImageDocumentAsync Tests

    [Fact]
    public async Task UploadImageDocumentAsync_WithValidImageFile_ShouldUploadAndReturnDto()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, FormFileHelper.CreateValidImageFile());
        var uploadResult = DocumentServiceFixture.CreateSuccessUploadResult();
        var document = DocumentServiceFixture.CreateTestDocument(lessonId: lessonId);
        var expectedDto = DocumentServiceFixture.CreateDocumentDto(document);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _mockFileService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);
        _mockDocumentRepository.Setup(x => x.InsertAsync(It.IsAny<Document>()))
            .ReturnsAsync((Document doc) => doc);
        _mockDocumentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockMapper.Setup(x => x.Map<DocumentDto>(It.IsAny<Document>()))
            .Returns(expectedDto);

        // Act
        var result = await _documentService.UploadImageDocumentAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);

        _mockLessonRepository.Verify(x => x.GetByIdAsync(lessonId), Times.Once);
        _mockFileService.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockDocumentRepository.Verify(x => x.InsertAsync(It.Is<Document>(d => 
            d.lessonId == lessonId && 
            d.documentName == uploadResult.PublicId &&
            d.fileUrl == uploadResult.SecureUrl)), Times.Once);
        _mockDocumentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UploadImageDocumentAsync_WithNonExistentLesson_ShouldThrowNotFoundException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadImageDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockFileService.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockDocumentRepository.Verify(x => x.InsertAsync(It.IsAny<Document>()), Times.Never);
    }

    [Fact]
    public async Task UploadImageDocumentAsync_WithNullFile_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var createDto = new CreateDocumentDto { lessonId = lessonId, file = null! };

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadImageDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_FILE");
        exception.Message.Should().Contain("File is required");

        _mockFileService.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadImageDocumentAsync_WithInvalidFileType_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var invalidFile = FormFileHelper.CreateInvalidFile(); // text/plain
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, invalidFile);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadImageDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_IMAGE_TYPE");
        exception.Message.Should().Contain("Only image files are allowed");

        _mockFileService.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadImageDocumentAsync_WithFileTooLarge_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var oversizedFile = FormFileHelper.CreateOversizedImageFile(); // > 50MB
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, oversizedFile);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadImageDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("FILE_TOO_LARGE");
        exception.Message.Should().Contain("exceeds maximum limit of 50MB");

        _mockFileService.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadImageDocumentAsync_WhenUploadFails_ShouldThrowInternalServerError()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId);
        var failedUploadResult = DocumentServiceFixture.CreateFailedUploadResult();

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _mockFileService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedUploadResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadImageDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("UPLOAD_FAILED");

        _mockDocumentRepository.Verify(x => x.InsertAsync(It.IsAny<Document>()), Times.Never);
    }

    [Fact]
    public async Task UploadImageDocumentAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId);
        var uploadResult = DocumentServiceFixture.CreateSuccessUploadResult();

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _mockFileService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);
        _mockDocumentRepository.Setup(x => x.InsertAsync(It.IsAny<Document>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadImageDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("UPLOAD_FAILED");
        exception.Message.Should().Contain("Failed to upload image document");
    }

    #endregion

    #region UploadVideoDocumentAsync Tests

    [Fact]
    public async Task UploadVideoDocumentAsync_WithValidVideoFile_ShouldUploadAndReturnDto()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, FormFileHelper.CreateValidVideoFile());
        var uploadResult = DocumentServiceFixture.CreateSuccessUploadResult();
        var document = DocumentServiceFixture.CreateTestDocument(lessonId: lessonId);
        var expectedDto = DocumentServiceFixture.CreateDocumentDto(document);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _mockFileService.Setup(x => x.UploadVideoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);
        _mockDocumentRepository.Setup(x => x.InsertAsync(It.IsAny<Document>()))
            .ReturnsAsync((Document doc) => doc);
        _mockDocumentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockMapper.Setup(x => x.Map<DocumentDto>(It.IsAny<Document>()))
            .Returns(expectedDto);

        // Act
        var result = await _documentService.UploadVideoDocumentAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);

        _mockFileService.Verify(x => x.UploadVideoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadVideoDocumentAsync_WithInvalidFileType_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var invalidFile = FormFileHelper.CreateInvalidFile();
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, invalidFile);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadVideoDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_VIDEO_TYPE");
        exception.Message.Should().Contain("Only video files are allowed");
    }

    [Fact]
    public async Task UploadVideoDocumentAsync_WithFileTooLarge_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var oversizedFile = FormFileHelper.CreateOversizedVideoFile(); // > 500MB
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, oversizedFile);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadVideoDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("FILE_TOO_LARGE");
        exception.Message.Should().Contain("exceeds maximum limit of 500MB");
    }

    #endregion

    #region UploadRawDocumentAsync Tests

    [Fact]
    public async Task UploadRawDocumentAsync_WithValidDocumentFile_ShouldUploadAndReturnDto()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, FormFileHelper.CreateValidPdfFile());
        var uploadResult = DocumentServiceFixture.CreateSuccessUploadResult();
        var document = DocumentServiceFixture.CreateTestDocument(lessonId: lessonId);
        var expectedDto = DocumentServiceFixture.CreateDocumentDto(document);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _mockFileService.Setup(x => x.UploadDocumentAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);
        _mockDocumentRepository.Setup(x => x.InsertAsync(It.IsAny<Document>()))
            .ReturnsAsync((Document doc) => doc);
        _mockDocumentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockMapper.Setup(x => x.Map<DocumentDto>(It.IsAny<Document>()))
            .Returns(expectedDto);

        // Act
        var result = await _documentService.UploadRawDocumentAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);

        _mockFileService.Verify(x => x.UploadDocumentAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadRawDocumentAsync_WithInvalidFileType_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var invalidFile = FormFileHelper.CreateInvalidFile();
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, invalidFile);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadRawDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_DOCUMENT_TYPE");
        exception.Message.Should().Contain("Only document files");
    }

    [Fact]
    public async Task UploadRawDocumentAsync_WithFileTooLarge_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var lesson = DocumentServiceFixture.CreateTestLesson(lessonId);
        var oversizedFile = FormFileHelper.CreateOversizedDocumentFile(); // > 100MB
        var createDto = DocumentServiceFixture.ValidCreateDto(lessonId, oversizedFile);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.UploadRawDocumentAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("FILE_TOO_LARGE");
        exception.Message.Should().Contain("exceeds maximum limit of 100MB");
    }

    #endregion

    #region GetDocumentByIdAsync Tests

    [Fact]
    public async Task GetDocumentByIdAsync_WithExistingId_ShouldReturnDocument()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var document = DocumentServiceFixture.CreateTestDocument(documentId);
        var expectedDto = DocumentServiceFixture.CreateDocumentDto(document);

        _mockDocumentRepository.Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync(document);
        _mockMapper.Setup(x => x.Map<DocumentDto>(document))
            .Returns(expectedDto);

        // Act
        var result = await _documentService.GetDocumentByIdAsync(documentId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);

        _mockDocumentRepository.Verify(x => x.GetByIdAsync(documentId), Times.Once);
        _mockMapper.Verify(x => x.Map<DocumentDto>(document), Times.Once);
    }

    [Fact]
    public async Task GetDocumentByIdAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var documentId = Guid.NewGuid();

        _mockDocumentRepository.Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync((Document?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.GetDocumentByIdAsync(documentId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockMapper.Verify(x => x.Map<DocumentDto>(It.IsAny<Document>()), Times.Never);
    }

    #endregion

    #region GetDocumentsByLessonIdAsync Tests

    [Fact]
    public async Task GetDocumentsByLessonIdAsync_WithValidLessonId_ShouldReturnDocuments()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var documents = DocumentServiceFixture.CreateDocumentList(lessonId, 3);
        var expectedDtos = documents.Select(DocumentServiceFixture.CreateDocumentDto).ToList();

        _mockDocumentRepository.Setup(x => x.GetDocumentsByLessonIdAsync(lessonId))
            .ReturnsAsync(documents);
        _mockMapper.Setup(x => x.Map<ICollection<DocumentDto>>(documents))
            .Returns(expectedDtos);

        // Act
        var result = await _documentService.GetDocumentsByLessonIdAsync(lessonId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expectedDtos);

        _mockDocumentRepository.Verify(x => x.GetDocumentsByLessonIdAsync(lessonId), Times.Once);
    }

    [Fact]
    public async Task GetDocumentsByLessonIdAsync_WithNoDocuments_ShouldReturnEmptyCollection()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var emptyDocuments = new List<Document>();
        var emptyDtos = new List<DocumentDto>();

        _mockDocumentRepository.Setup(x => x.GetDocumentsByLessonIdAsync(lessonId))
            .ReturnsAsync(emptyDocuments);
        _mockMapper.Setup(x => x.Map<ICollection<DocumentDto>>(emptyDocuments))
            .Returns(emptyDtos);

        // Act
        var result = await _documentService.GetDocumentsByLessonIdAsync(lessonId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region DeleteDocumentAsync Tests

    [Fact]
    public async Task DeleteDocumentAsync_WithExistingDocument_ShouldDeleteFromStorageAndDatabase()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var document = DocumentServiceFixture.CreateTestDocument(documentId);
        var deletionResult = DocumentServiceFixture.CreateSuccessDeletionResult();

        _mockDocumentRepository.Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync(document);
        _mockFileService.Setup(x => x.DeleteFileByUrlAsync(document.fileUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletionResult);
        _mockDocumentRepository.Setup(x => x.DeleteAsync(document))
            .Returns(Task.CompletedTask);
        _mockDocumentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _documentService.DeleteDocumentAsync(documentId);

        // Assert
        result.Should().BeTrue();

        _mockDocumentRepository.Verify(x => x.GetByIdAsync(documentId), Times.Once);
        _mockFileService.Verify(x => x.DeleteFileByUrlAsync(document.fileUrl, It.IsAny<CancellationToken>()), Times.Once);
        _mockDocumentRepository.Verify(x => x.DeleteAsync(document), Times.Once);
        _mockDocumentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteDocumentAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var documentId = Guid.NewGuid();

        _mockDocumentRepository.Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync((Document?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.DeleteDocumentAsync(documentId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockFileService.Verify(x => x.DeleteFileByUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockDocumentRepository.Verify(x => x.DeleteAsync(It.IsAny<Document>()), Times.Never);
    }

    [Fact]
    public async Task DeleteDocumentAsync_WhenFileServiceFails_ShouldStillDeleteFromDatabase()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var document = DocumentServiceFixture.CreateTestDocument(documentId);
        var failedDeletionResult = DocumentServiceFixture.CreateFailedDeletionResult();

        _mockDocumentRepository.Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync(document);
        _mockFileService.Setup(x => x.DeleteFileByUrlAsync(document.fileUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedDeletionResult);
        _mockFileService.Setup(x => x.DeleteFileAsync(document.documentName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedDeletionResult);
        _mockDocumentRepository.Setup(x => x.DeleteAsync(document))
            .Returns(Task.CompletedTask);
        _mockDocumentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _documentService.DeleteDocumentAsync(documentId);

        // Assert
        result.Should().BeTrue();

        // Should try both URL-based and PublicId-based deletion
        _mockFileService.Verify(x => x.DeleteFileByUrlAsync(document.fileUrl, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileService.Verify(x => x.DeleteFileAsync(document.documentName, It.IsAny<CancellationToken>()), Times.Once);
        _mockDocumentRepository.Verify(x => x.DeleteAsync(document), Times.Once);
    }

    [Fact]
    public async Task DeleteDocumentAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var document = DocumentServiceFixture.CreateTestDocument(documentId);
        var deletionResult = DocumentServiceFixture.CreateSuccessDeletionResult();

        _mockDocumentRepository.Setup(x => x.GetByIdAsync(documentId))
            .ReturnsAsync(document);
        _mockFileService.Setup(x => x.DeleteFileByUrlAsync(document.fileUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletionResult);
        _mockDocumentRepository.Setup(x => x.DeleteAsync(document))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _documentService.DeleteDocumentAsync(documentId));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("DELETE_FAILED");
        exception.Message.Should().Contain("Failed to delete document");
    }

    #endregion
}
