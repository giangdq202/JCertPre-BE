
namespace JCertPreApplication.Application.Dtos.Question
{
    public class ImportQuestionsResultDto
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<ImportQuestionErrorDto> FailedQuestions { get; set; } = new();
        public string? FailedFileUrl { get; set; }
    }

    public class ImportQuestionErrorDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}