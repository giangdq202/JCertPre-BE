using System;

public class TestQuestionDto
{
    public Guid TestQuestionId { get; set; }
    public Guid TestId { get; set; }
    public Guid QuestionId { get; set; }
    public bool IsActive { get; set; }
    public QuestionInTestDto? Question { get; set; }
}