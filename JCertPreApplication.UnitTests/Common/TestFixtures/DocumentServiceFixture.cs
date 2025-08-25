using AutoMapper;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Dtos.File;
using JCertPreApplication.Application.Features.Documents;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class DocumentServiceFixture
{
    public DocumentService DocumentService { get; }
    public Mock<IDocumentRepository> MockDocumentRepository { get; }
    public Mock<ILessonRepository> MockLessonRepository { get; }
    public Mock<IFileService> MockFileService { get; }
    public Mock<IMapper> MockMapper { get; }

    public DocumentServiceFixture()
    {
        MockDocumentRepository = new Mock<IDocumentRepository>();
        MockLessonRepository = new Mock<ILessonRepository>();
        MockFileService = new Mock<IFileService>();
        MockMapper = new Mock<IMapper>();

        DocumentService = new DocumentService(
            MockDocumentRepository.Object,
            MockLessonRepository.Object,
            MockFileService.Object,
            MockMapper.Object);
    }

    public static Lesson CreateTestLesson(Guid? lessonId = null)
    {
        return LessonBuilder.Create()
            .WithId(lessonId ?? Guid.NewGuid())
            .WithTitle("Test Lesson")
            .Build();
    }

    public static Document CreateTestDocument(Guid? documentId = null, Guid? lessonId = null)
    {
        return DocumentBuilder.Create()
            .WithId(documentId ?? Guid.NewGuid())
            .WithLessonId(lessonId ?? Guid.NewGuid())
            .WithName("test-document")
            .WithUrl("https://example.com/document.jpg")
            .Build();
    }

    public static List<Document> CreateDocumentList(Guid lessonId, int count)
    {
        var documents = new List<Document>();
        for (int i = 1; i <= count; i++)
        {
            documents.Add(DocumentBuilder.Create()
                .WithLessonId(lessonId)
                .WithName($"document-{i}")
                .WithUrl($"https://example.com/document-{i}.jpg")
                .Build());
        }
        return documents;
    }

    public static CreateDocumentDto ValidCreateDto(Guid? lessonId = null, IFormFile? file = null)
    {
        return new CreateDocumentDto
        {
            lessonId = lessonId ?? Guid.NewGuid(),
            file = file ?? FormFileHelper.CreateValidImageFile()
        };
    }

    public static DocumentDto CreateDocumentDto(Document document)
    {
        return new DocumentDto
        {
            documentId = document.documentId,
            lessonId = document.lessonId,
            documentName = document.documentName,
            fileUrl = document.fileUrl,
            uploadedAt = document.uploadedAt
        };
    }

    public static FileUploadResult CreateSuccessUploadResult(string publicId = "test-id", string url = "https://example.com/file.jpg")
    {
        return new FileUploadResult
        {
            Success = true,
            PublicId = publicId,
            Url = url,
            SecureUrl = $"https://secure.example.com/{publicId}.jpg",
            Bytes = 1024,
            Format = "jpg",
            ResourceType = "image",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static FileUploadResult CreateFailedUploadResult(string errorMessage = "Upload failed")
    {
        return new FileUploadResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            PublicId = string.Empty,
            Url = string.Empty
        };
    }

    public static FileDeletionResult CreateSuccessDeletionResult()
    {
        return new FileDeletionResult
        {
            Success = true
        };
    }

    public static FileDeletionResult CreateFailedDeletionResult(string errorMessage = "Deletion failed")
    {
        return new FileDeletionResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
