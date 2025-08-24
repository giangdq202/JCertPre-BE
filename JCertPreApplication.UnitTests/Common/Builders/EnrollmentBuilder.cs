using JCertPreApplication.Domain.Entities;
using System;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class EnrollmentBuilder
{
    private Enrollment _enrollment;

    public EnrollmentBuilder()
    {
        _enrollment = new Enrollment
        {
            enrollmentId = Guid.NewGuid(),
            userId = Guid.NewGuid(),
            courseId = Guid.NewGuid(),
            enrollDate = DateTime.UtcNow,
            price = 1000000m
        };
    }

    public static EnrollmentBuilder Create() => new EnrollmentBuilder();

    public EnrollmentBuilder WithId(Guid enrollmentId)
    {
        _enrollment.enrollmentId = enrollmentId;
        return this;
    }

    public EnrollmentBuilder WithUserId(Guid userId)
    {
        _enrollment.userId = userId;
        return this;
    }

    public EnrollmentBuilder WithCourseId(Guid courseId)
    {
        _enrollment.courseId = courseId;
        return this;
    }

    public EnrollmentBuilder WithEnrollDate(DateTime enrollDate)
    {
        _enrollment.enrollDate = enrollDate;
        return this;
    }

    public EnrollmentBuilder WithPrice(decimal price)
    {
        _enrollment.price = price;
        return this;
    }

    public EnrollmentBuilder WithCourse(Course course)
    {
        _enrollment.Course = course;
        return this;
    }

    public Enrollment Build() => _enrollment;
}