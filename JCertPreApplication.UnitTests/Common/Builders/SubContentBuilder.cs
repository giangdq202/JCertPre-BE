using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class SubContentBuilder
{
    private SubContent _subContent;

    public SubContentBuilder()
    {
        _subContent = new SubContent
        {
            SubContentId = Guid.NewGuid(),
            SubContentName = SubContentName.Mondai1,
            Level = CourseLevel.N5,
            ContentName = ContentName.Kanji
        };
    }

    public static SubContentBuilder Create() => new SubContentBuilder();

    public SubContentBuilder WithId(Guid id)
    {
        _subContent.SubContentId = id;
        return this;
    }

    public SubContentBuilder WithSubContentName(SubContentName subContentName)
    {
        _subContent.SubContentName = subContentName;
        return this;
    }

    public SubContentBuilder WithLevel(CourseLevel level)
    {
        _subContent.Level = level;
        return this;
    }

    public SubContentBuilder WithContentName(ContentName contentName)
    {
        _subContent.ContentName = contentName;
        return this;
    }

    public SubContentBuilder WithSubContentId(Guid subContentId)
    {
        _subContent.SubContentId = subContentId;
        return this;
    }

    public SubContent Build() => _subContent;
}