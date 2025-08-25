using JCertPreApplication.Domain.Entities;
using System;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class DocumentBuilder
{
    private Document _document;

    public DocumentBuilder()
    {
        _document = new Document
        {
            documentId = Guid.NewGuid(),
            lessonId = Guid.NewGuid(),
            documentName = "test-document",
            fileUrl = "https://example.com/document.jpg",
            uploadedAt = DateTime.UtcNow
        };
    }

    public static DocumentBuilder Create() => new DocumentBuilder();

    public DocumentBuilder WithId(Guid id)
    {
        _document.documentId = id;
        return this;
    }

    public DocumentBuilder WithLessonId(Guid lessonId)
    {
        _document.lessonId = lessonId;
        return this;
    }

    public DocumentBuilder WithName(string name)
    {
        _document.documentName = name;
        return this;
    }

    public DocumentBuilder WithUrl(string url)
    {
        _document.fileUrl = url;
        return this;
    }

    public DocumentBuilder WithUploadedAt(DateTime uploadedAt)
    {
        _document.uploadedAt = uploadedAt;
        return this;
    }

    public Document Build() => _document;
}
