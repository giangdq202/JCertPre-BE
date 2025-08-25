using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class StudyPlanItemBuilder
{
    private StudyPlanItem _item;

    public StudyPlanItemBuilder()
    {
        _item = new StudyPlanItem
        {
            itemId = Guid.NewGuid(),
            planId = Guid.NewGuid(),
            sequence = 1,
            itemType = "Course",
            courseId = Guid.NewGuid(),
            TestTemplateTypeId = null,
            description = "Test Study Plan Item",
            status = ItemStatus.Pending
        };
    }

    public static StudyPlanItemBuilder Create() => new StudyPlanItemBuilder();

    public StudyPlanItemBuilder WithId(Guid id)
    {
        _item.itemId = id;
        return this;
    }

    public StudyPlanItemBuilder WithPlanId(Guid planId)
    {
        _item.planId = planId;
        return this;
    }

    public StudyPlanItemBuilder WithSequence(int sequence)
    {
        _item.sequence = sequence;
        return this;
    }

    public StudyPlanItemBuilder WithItemType(string itemType)
    {
        _item.itemType = itemType;
        return this;
    }

    public StudyPlanItemBuilder WithCourseId(Guid? courseId)
    {
        _item.courseId = courseId;
        return this;
    }

    public StudyPlanItemBuilder WithTestId(Guid? testId)
    {
        _item.TestTemplateTypeId = testId;
        return this;
    }

    public StudyPlanItemBuilder WithDescription(string? description)
    {
        _item.description = description;
        return this;
    }

    public StudyPlanItemBuilder WithStatus(ItemStatus status)
    {
        _item.status = status;
        return this;
    }

    public StudyPlanItem Build() => _item;
}
