using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Domain.Enums;
namespace JCertPreApplication.Application.Dtos.SubContent;
public class UpdateSubContentDto
{
    [Required(ErrorMessage = "SubContentName is required.")]
    public SubContentName SubContentName { get; set; }

    [Required(ErrorMessage = "Level is required.")]
    public CourseLevel Level { get; set; }

    [Required(ErrorMessage = "ContentName is required.")]
    public ContentName ContentName { get; set; }
}