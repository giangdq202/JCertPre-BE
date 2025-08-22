using JCertPreApplication.Application.Dtos.SubContent;
using JCertPreApplication.Application.Dtos.Utilities;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
namespace JCertPreApplication.Application.Features.SubContents;
public interface ISubContentService
{
    Task<Pagination<SubContent>> GetAllAsync(string? search, CourseLevel? level, ContentName? contentName, SubContentName? subContentName, int pageIndex, int pageSize);
    Task<SubContent> CreateAsync(CreateSubContentDto dto);
    Task<SubContent> UpdateAsync(Guid subContentId, UpdateSubContentDto dto);
    Task DeleteAsync(Guid subContentId);

    Task<List<EnumValueDto>> GetSubContentNameEnumValuesAsync();
    Task<List<EnumValueDto>> GetLevelEnumValuesAsync();
    Task<List<EnumValueDto>> GetContentNameEnumValuesAsync();
}