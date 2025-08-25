using JCertPreApplication.Application.Dtos.SubContent;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class UpdateSubContentDtoBuilder
{
    private UpdateSubContentDto _dto;

    public UpdateSubContentDtoBuilder()
    {
        _dto = new UpdateSubContentDto
        {
            SubContentName = SubContentName.Mondai2,
            Level = CourseLevel.N4,
            ContentName = ContentName.Vocabulary
        };
    }

    public static UpdateSubContentDtoBuilder Create() => new UpdateSubContentDtoBuilder();

    public UpdateSubContentDtoBuilder WithSubContentName(SubContentName subContentName)
    {
        _dto.SubContentName = subContentName;
        return this;
    }

    public UpdateSubContentDtoBuilder WithLevel(CourseLevel level)
    {
        _dto.Level = level;
        return this;
    }

    public UpdateSubContentDtoBuilder WithContentName(ContentName contentName)
    {
        _dto.ContentName = contentName;
        return this;
    }

    public UpdateSubContentDto Build() => _dto;
}
