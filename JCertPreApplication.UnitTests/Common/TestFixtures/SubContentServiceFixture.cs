using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.SubContents;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

/// <summary>
/// Test fixture for SubContentService providing mocked dependencies and helper methods
/// </summary>
public class SubContentServiceFixture
{
    public SubContentService SubContentService { get; }
    public Mock<IGenericRepository<SubContent>> MockRepository { get; }

    public SubContentServiceFixture()
    {
        MockRepository = new Mock<IGenericRepository<SubContent>>();
        SubContentService = new SubContentService(MockRepository.Object);
    }

    /// <summary>
    /// Creates a sample SubContent for testing
    /// </summary>
    public SubContent CreateSampleSubContent(SubContentName? subContentName = null, CourseLevel? level = null, ContentName? contentName = null)
    {
        return SubContentBuilder.Create()
            .WithSubContentName(subContentName ?? SubContentName.Mondai1)
            .WithLevel(level ?? CourseLevel.N5)
            .WithContentName(contentName ?? ContentName.Kanji)
            .Build();
    }

    /// <summary>
    /// Creates a list of sample SubContents with different enum values for testing filters
    /// </summary>
    public List<SubContent> CreateSampleSubContents()
    {
        return new List<SubContent>
        {
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai1)
                .WithLevel(CourseLevel.N5)
                .WithContentName(ContentName.Kanji)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai3)
                .WithLevel(CourseLevel.N4)
                .WithContentName(ContentName.Vocabulary)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai5)
                .WithLevel(CourseLevel.N3)
                .WithContentName(ContentName.Grammar)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai8)
                .WithLevel(CourseLevel.N2)
                .WithContentName(ContentName.Reading)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai11)
                .WithLevel(CourseLevel.N1)
                .WithContentName(ContentName.Listening)
                .Build()
        };
    }

    /// <summary>
    /// Creates a pagination result for testing
    /// </summary>
    public Pagination<SubContent> CreatePaginationResult(List<SubContent> items, int pageIndex = 1, int pageSize = 10)
    {
        return new Pagination<SubContent>
        {
            Items = items,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = items.Count
        };
    }
}
