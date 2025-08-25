using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

/// <summary>
/// Test fixture for StudyPlanItemService providing mocked dependencies and helper methods
/// </summary>
public class StudyPlanItemServiceFixture
{
    public StudyPlanItemService StudyPlanItemService { get; }
    public Mock<IStudyPlanItemRepository> MockStudyPlanItemRepository { get; }
    public Mock<IStudyPlanRepository> MockStudyPlanRepository { get; }

    public StudyPlanItemServiceFixture()
    {
        MockStudyPlanItemRepository = new Mock<IStudyPlanItemRepository>();
        MockStudyPlanRepository = new Mock<IStudyPlanRepository>();
        StudyPlanItemService = new StudyPlanItemService(
            MockStudyPlanItemRepository.Object,
            MockStudyPlanRepository.Object
        );
    }

    /// <summary>
    /// Creates a valid StudyPlanItem for testing
    /// </summary>
    public static StudyPlanItem ValidStudyPlanItem()
    {
        return StudyPlanItemBuilder.Create().Build();
    }

    /// <summary>
    /// Creates a valid StudyPlanItem with specific planId
    /// </summary>
    public static StudyPlanItem ValidStudyPlanItemWithPlanId(Guid planId)
    {
        return StudyPlanItemBuilder.Create()
            .WithPlanId(planId)
            .Build();
    }

    /// <summary>
    /// Creates a list of StudyPlanItems for the same plan
    /// </summary>
    public static List<StudyPlanItem> CreateMultipleItems(Guid planId, int count = 3)
    {
        var items = new List<StudyPlanItem>();
        for (int i = 0; i < count; i++)
        {
            items.Add(StudyPlanItemBuilder.Create()
                .WithPlanId(planId)
                .WithSequence(i + 1)
                .WithItemType(i % 2 == 0 ? "Course" : "Test")
                .Build());
        }
        return items;
    }

    /// <summary>
    /// Creates a valid UpdateStudyPlanItemDto for testing
    /// </summary>
    public static UpdateStudyPlanItemDto ValidUpdateDto()
    {
        return UpdateStudyPlanItemDtoBuilder.Create().Build();
    }

    /// <summary>
    /// Creates a partial UpdateStudyPlanItemDto with only some fields set
    /// </summary>
    public static UpdateStudyPlanItemDto PartialUpdateDto()
    {
        return UpdateStudyPlanItemDtoBuilder.Create()
            .WithSequence(5)
            .WithItemType(null)
            .WithCourseId(null)
            .WithTestId(null)
            .WithStatus(null)
            .Build();
    }

    /// <summary>
    /// Creates an UpdateStudyPlanItemDto with empty/whitespace ItemType
    /// </summary>
    public static UpdateStudyPlanItemDto UpdateDtoWithEmptyItemType()
    {
        return UpdateStudyPlanItemDtoBuilder.Create()
            .WithItemType("")
            .Build();
    }

    /// <summary>
    /// Creates an UpdateStudyPlanItemDto with whitespace ItemType
    /// </summary>
    public static UpdateStudyPlanItemDto UpdateDtoWithWhitespaceItemType()
    {
        return UpdateStudyPlanItemDtoBuilder.Create()
            .WithItemType("   ")
            .Build();
    }

    /// <summary>
    /// Creates a valid StudyPlan for validation
    /// </summary>
    public static StudyPlan ValidStudyPlan()
    {
        return StudyPlanBuilder.Create().Build();
    }
}
