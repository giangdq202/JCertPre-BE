using JCertPreApplication.Application.Dtos.Question;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class ExplanationRequestDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<GeneratedChoiceDto> Choices { get; set; } = new();
    }

    public class ExplanationResponseDto
    {
        public string Explanation { get; set; } = string.Empty;
    }
}
