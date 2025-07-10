using System;

public class ChoiceReadDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public bool IsCorrect { get; set; }
    public Guid QuestionId { get; set; }
}