using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class LivestreamBuilder
{
    private Livestream _livestream;

    public LivestreamBuilder()
    {
        _livestream = new Livestream
        {
            livestreamId = Guid.NewGuid(),
            courseId = Guid.NewGuid(),
            description = "Test Livestream",
            scheduledDateTime = DateTime.UtcNow.AddHours(1),
            durationMinutes = 60,
            status = LivestreamStatus.SCHEDULED
        };
    }

    public static LivestreamBuilder Create() => new LivestreamBuilder();

    public LivestreamBuilder WithId(Guid id)
    {
        _livestream.livestreamId = id;
        return this;
    }

    public LivestreamBuilder WithCourseId(Guid courseId)
    {
        _livestream.courseId = courseId;
        return this;
    }

    public LivestreamBuilder WithDescription(string description)
    {
        _livestream.description = description;
        return this;
    }

    public LivestreamBuilder WithSchedule(DateTime schedule)
    {
        _livestream.scheduledDateTime = schedule;
        return this;
    }

    public LivestreamBuilder WithDuration(int minutes)
    {
        _livestream.durationMinutes = minutes;
        return this;
    }

    public LivestreamBuilder AsScheduled()
    {
        _livestream.status = LivestreamStatus.SCHEDULED;
        return this;
    }

    public LivestreamBuilder AsLive()
    {
        _livestream.status = LivestreamStatus.LIVE;
        return this;
    }

    public LivestreamBuilder AsCompleted()
    {
        _livestream.status = LivestreamStatus.COMPLETED;
        return this;
    }

    public LivestreamBuilder WithCourse(Course course)
    {
        _livestream.Course = course;
        _livestream.courseId = course.courseId;
        return this;
    }

    public LivestreamBuilder WithStatus(LivestreamStatus status)
    {
        _livestream.status = status;
        return this;
    }

    public Livestream Build() => _livestream;
}
