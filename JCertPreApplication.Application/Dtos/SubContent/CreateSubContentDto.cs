using JCertPreApplication.Domain.Enums;

public class CreateSubContentDto
{
    public SubContentName SubContentName { get; set; }
    public CourseLevel Level { get; set; }
    public ContentName ContentName { get; set; }
}