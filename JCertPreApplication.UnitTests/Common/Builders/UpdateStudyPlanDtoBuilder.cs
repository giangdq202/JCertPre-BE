using JCertPreApplication.Application.Dtos.StudyPlan;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class UpdateStudyPlanDtoBuilder
{
    private UpdateStudyPlanDto _updateDto;

    public UpdateStudyPlanDtoBuilder()
    {
        _updateDto = new UpdateStudyPlanDto
        {
            PlanName = "Updated Plan Name",
            Description = "Updated Description",
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(31),
            StudentId = Guid.NewGuid()
        };
    }

    public static UpdateStudyPlanDtoBuilder Create() => new UpdateStudyPlanDtoBuilder();

    public UpdateStudyPlanDtoBuilder WithPlanName(string? planName)
    {
        _updateDto.PlanName = planName;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithDescription(string? description)
    {
        _updateDto.Description = description;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithStartDate(DateTime? startDate)
    {
        _updateDto.StartDate = startDate;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithEndDate(DateTime? endDate)
    {
        _updateDto.EndDate = endDate;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithStudentId(Guid? studentId)
    {
        _updateDto.StudentId = studentId;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithPartialData()
    {
        _updateDto.PlanName = "Partial Update Name";
        _updateDto.Description = null;
        _updateDto.StartDate = null;
        _updateDto.EndDate = null;
        _updateDto.StudentId = null;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithOnlyStudentId(Guid studentId)
    {
        _updateDto.PlanName = null;
        _updateDto.Description = null;
        _updateDto.StartDate = null;
        _updateDto.EndDate = null;
        _updateDto.StudentId = studentId;
        return this;
    }

    public UpdateStudyPlanDtoBuilder WithAllNullValues()
    {
        _updateDto.PlanName = null;
        _updateDto.Description = null;
        _updateDto.StartDate = null;
        _updateDto.EndDate = null;
        _updateDto.StudentId = null;
        return this;
    }

    public UpdateStudyPlanDto Build() => _updateDto;

    public static UpdateStudyPlanDto Default() => Create().Build();
}
