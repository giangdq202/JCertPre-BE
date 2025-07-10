using System;
using System.Collections.Generic;

public class QuestionInTestDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public string? Explanation { get; set; }
    public int Points { get; set; }
    public string QuestionType { get; set; } = null!;
    public List<ChoiceReadDto>? Choices { get; set; }
}