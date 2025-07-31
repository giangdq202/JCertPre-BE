using JCertPreApplication.Application.Dtos.QuestionAttachment;

public class QuestionForTestDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public string? QuestionType { get; set; }
    public List<ChoiceForTestDto>? Choices { get; set; }
    public List<QuestionAttachmentDto>? QuestionAttachments { get; set; }
}

public class ChoiceForTestDto
{
    public Guid ChoiceId { get; set; }
    public string? Content { get; set; }
}