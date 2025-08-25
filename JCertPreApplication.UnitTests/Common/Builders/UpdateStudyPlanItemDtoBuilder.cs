using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class UpdateStudyPlanItemDtoBuilder
{
    private UpdateStudyPlanItemDto _dto;

    public UpdateStudyPlanItemDtoBuilder()
    {
        _dto = new UpdateStudyPlanItemDto
        {
            Sequence = 2,
            ItemType = "Test",
            CourseId = Guid.NewGuid(),
            TestTemplateTypeId = Guid.NewGuid(),
            Status = ItemStatus.InProgress
        };
    }

    public static UpdateStudyPlanItemDtoBuilder Create() => new UpdateStudyPlanItemDtoBuilder();

    public UpdateStudyPlanItemDtoBuilder WithSequence(int? sequence)
    {
        _dto.Sequence = sequence;
        return this;
    }

    public UpdateStudyPlanItemDtoBuilder WithItemType(string? itemType)
    {
        _dto.ItemType = itemType;
        return this;
    }

    public UpdateStudyPlanItemDtoBuilder WithCourseId(Guid? courseId)
    {
        _dto.CourseId = courseId;
        return this;
    }

    public UpdateStudyPlanItemDtoBuilder WithTestId(Guid? testId)
    {
        _dto.TestTemplateTypeId = testId;
        return this;
    }

    public UpdateStudyPlanItemDtoBuilder WithStatus(ItemStatus? status)
    {
        _dto.Status = status;
        return this;
    }

    public UpdateStudyPlanItemDto Build() => _dto;
}
