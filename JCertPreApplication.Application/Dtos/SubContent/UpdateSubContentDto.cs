using JCertPreApplication.Domain.Enums;

public class UpdateSubContentDto
{
    public SubContentName SubContentName { get; set; }
    public CourseLevel Level { get; set; }
    public ContentName ContentName { get; set; }
}