using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class StudyPlanBuilder
{
    private StudyPlan _studyPlan;

    public StudyPlanBuilder()
    {
        _studyPlan = new StudyPlan
        {
            planId = Guid.NewGuid(),
            studentId = Guid.NewGuid(),
            createdByStaffId = Guid.NewGuid(),
            planName = "Test Study Plan",
            description = "Test Description",
            startDate = DateTime.UtcNow.Date,
            endDate = DateTime.UtcNow.Date.AddDays(30),
            StudyPlanItems = new List<StudyPlanItem>()
        };
    }

    public static StudyPlanBuilder Create() => new StudyPlanBuilder();

    public StudyPlanBuilder WithId(Guid id)
    {
        _studyPlan.planId = id;
        return this;
    }

    public StudyPlanBuilder WithStudentId(Guid studentId)
    {
        _studyPlan.studentId = studentId;
        return this;
    }

    public StudyPlanBuilder WithStaffId(Guid staffId)
    {
        _studyPlan.createdByStaffId = staffId;
        return this;
    }

    public StudyPlanBuilder WithName(string name)
    {
        _studyPlan.planName = name;
        return this;
    }

    public StudyPlanBuilder WithDescription(string description)
    {
        _studyPlan.description = description;
        return this;
    }

    public StudyPlanBuilder WithStartDate(DateTime startDate)
    {
        _studyPlan.startDate = startDate;
        return this;
    }

    public StudyPlanBuilder WithEndDate(DateTime endDate)
    {
        _studyPlan.endDate = endDate;
        return this;
    }

    public StudyPlanBuilder WithItems(ICollection<StudyPlanItem> items)
    {
        _studyPlan.StudyPlanItems = items;
        return this;
    }

    public StudyPlanBuilder WithEmptyItems()
    {
        _studyPlan.StudyPlanItems = new List<StudyPlanItem>();
        return this;
    }

    public StudyPlanBuilder WithOneItem()
    {
        _studyPlan.StudyPlanItems = new List<StudyPlanItem>
        {
            new StudyPlanItem { itemId = Guid.NewGuid() }
        };
        return this;
    }

    public StudyPlan Build() => _studyPlan;

    public static StudyPlan Default() => Create().Build();

    public static List<StudyPlan> CreateList(int count = 3)
    {
        var studyPlans = new List<StudyPlan>();
        for (int i = 0; i < count; i++)
        {
            studyPlans.Add(Create()
                .WithName($"Study Plan {i + 1}")
                .WithDescription($"Description {i + 1}")
                .Build());
        }
        return studyPlans;
    }

    public static List<StudyPlan> CreateListForStudent(Guid studentId, int count = 2)
    {
        var studyPlans = new List<StudyPlan>();
        for (int i = 0; i < count; i++)
        {
            studyPlans.Add(Create()
                .WithStudentId(studentId)
                .WithName($"Student Plan {i + 1}")
                .Build());
        }
        return studyPlans;
    }
}
