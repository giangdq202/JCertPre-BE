using JCertPreApplication.Application.Dtos.SubContent;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class CreateSubContentDtoBuilder
{
    private CreateSubContentDto _dto;

    public CreateSubContentDtoBuilder()
    {
        _dto = new CreateSubContentDto
        {
            SubContentName = SubContentName.Mondai1,
            Level = CourseLevel.N5,
            ContentName = ContentName.Kanji
        };
    }

    public static CreateSubContentDtoBuilder Create() => new CreateSubContentDtoBuilder();

    public CreateSubContentDtoBuilder WithSubContentName(SubContentName subContentName)
    {
        _dto.SubContentName = subContentName;
        return this;
    }

    public CreateSubContentDtoBuilder WithLevel(CourseLevel level)
    {
        _dto.Level = level;
        return this;
    }

    public CreateSubContentDtoBuilder WithContentName(ContentName contentName)
    {
        _dto.ContentName = contentName;
        return this;
    }

    public CreateSubContentDto Build() => _dto;
}
