using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class GenerateQuestionRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "JLPT Level is required.")]
        public string Level { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content name is required.")]
        public string ContentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Validate Level
            var validLevels = new[] { "N5", "N4", "N3", "N2", "N1" };
            if (!validLevels.Contains(Level, StringComparer.OrdinalIgnoreCase))
            {
                results.Add(new ValidationResult(
                    $"Level must be one of: {string.Join(", ", validLevels)}", 
                    new[] { nameof(Level) }));
            }

            // Validate ContentName
            var validContentNames = new[] { "Kanji", "Vocabulary", "Grammar", "Reading" };
            if (!validContentNames.Contains(ContentName, StringComparer.OrdinalIgnoreCase))
            {
                results.Add(new ValidationResult(
                    $"Content name must be one of: {string.Join(", ", validContentNames)}", 
                    new[] { nameof(ContentName) }));
            }

            // Validate Description - allowed values from SubContentName descriptions
            var validDescriptions = new[] { 
                "Đọc chữ Hán", "Nhớ chữ Hán", 
                "Chọn từ phù hợp với câu", "Tìm câu có cách diễn đạt giống",
                "Chọn ngữ pháp phù hợp với câu", "Sắp xếp câu", "Tìm đáp án đúng để hoàn thành đoạn văn",
                "Đoạn văn ngắn", "Trung văn", "Tìm kiếm thông tin",
                "Hiểu đề bài", "Hiểu điểm chính", "Diễn đạt bằng lời nói", "Phản hồi tức thời"
            };
            if (!validDescriptions.Contains(Description, StringComparer.OrdinalIgnoreCase))
            {
                results.Add(new ValidationResult(
                    $"Description must be one of: {string.Join(", ", validDescriptions)}", 
                    new[] { nameof(Description) }));
            }

            return results;
        }
    }
}
