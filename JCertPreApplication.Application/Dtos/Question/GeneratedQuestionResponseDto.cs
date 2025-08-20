namespace JCertPreApplication.Application.Dtos.Question
{
    public class GeneratedQuestionResponseDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public List<GeneratedChoiceDto> Choices { get; set; } = new();
    }

    public class GeneratedChoiceDto
    {
        public string ChoiceText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
