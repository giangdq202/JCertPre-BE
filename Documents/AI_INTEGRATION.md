# AI Integration

This document consolidates the AI features and configuration for question generation and explanation within the JCertPre backend.

---

## Overview

The AI integration supports generating JLPT-style questions with:
- A required Description field to specify the question type (per content category)
- An Explanation field in responses (Vietnamese) explaining the correct answer
- Enhanced prompts tailored to JLPT levels, content types, and specific question types

---

## Request DTOs and Response DTOs

### Request DTO for AI Question Generation
```csharp
public class GenerateQuestionRequestDto : IValidatableObject
{
    [Required] public string Level { get; set; }        // N5, N4, N3, N2, N1
    [Required] public string ContentName { get; set; }  // Kanji, Vocabulary, Grammar, Reading, Listening
    [Required] public string Description { get; set; }  // Question type description
}
```

### Response DTO with Explanation
```csharp
public class GeneratedQuestionResponseDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;  // Vietnamese explanation
    public List<GeneratedChoiceDto> Choices { get; set; } = new();
}
```

---

## Available Description Values

```
# Kanji:
- "Đọc chữ Hán"
- "Nhớ chữ Hán"

# Vocabulary:
- "Chọn từ phù hợp với câu"
- "Tìm câu có cách diễn đạt giống"

# Grammar:
- "Chọn ngữ pháp phù hợp với câu"
- "Sắp xếp câu"
- "Tìm đáp án đúng để hoàn thành đoạn văn"

# Reading:
- "Đoạn văn ngắn"
- "Trung văn"
- "Tìm kiếm thông tin"

# Listening:
- "Hiểu đề bài"
- "Hiểu điểm chính"
- "Diễn đạt bằng lời nói"
- "Phản hồi tức thời"
```

---

## Enhanced AI Prompt

### Prompt Structure
```
Context:
- JLPT Level: "N3"
- Content Type: "Grammar"
- Question Type: "Chọn ngữ pháp phù hợp với câu"

Question Type Guidelines:
- Create a sentence with a grammar point to test
- Provide grammar pattern options
- Test specific grammar structures for the JLPT level
- Include common grammar mistakes as wrong options
```

### Benefits
- More specific questions aligned to exact question types
- Better JLPT alignment following official patterns
- Improved accuracy via type-specific guidelines
- Consistent quality through standardized formats

---

## API Usage Examples

### Example 1: Grammar Question
```bash
POST /api/questions/generate-ai
{
  "level": "N3",
  "contentName": "Grammar",
  "description": "Chọn ngữ pháp phù hợp với câu"
}
```
Expected Response:
```json
{
  "questionText": "次の文の（　）に入る最も適切な語を選びなさい。\n彼は毎日一生懸命（　）勉強している。",
  "choices": [
    { "choiceText": "に", "isCorrect": true },
    { "choiceText": "で", "isCorrect": false },
    { "choiceText": "を", "isCorrect": false },
    { "choiceText": "が", "isCorrect": false }
  ]
}
```

### Example 2: Vocabulary Question
```bash
POST /api/questions/generate-ai
{
  "level": "N4",
  "contentName": "Vocabulary",
  "description": "Chọn từ phù hợp với câu"
}
```
Expected Response:
```json
{
  "questionText": "次の文の（　）に入る最も適切な語を選びなさい。\n今日は天気が（　）、散歩に行きましょう。",
  "choices": [
    { "choiceText": "いい", "isCorrect": true },
    { "choiceText": "悪い", "isCorrect": false },
    { "choiceText": "高い", "isCorrect": false },
    { "choiceText": "低い", "isCorrect": false }
  ]
}
```

### Example 3: Kanji Question
```bash
POST /api/questions/generate-ai
{
  "level": "N5",
  "contentName": "Kanji",
  "description": "Đọc chữ Hán"
}
```
Expected Response:
```json
{
  "questionText": "次の漢字の読み方として正しいものを選びなさい。\n学校",
  "choices": [
    { "choiceText": "がっこう", "isCorrect": true },
    { "choiceText": "がくこう", "isCorrect": false },
    { "choiceText": "がくしょう", "isCorrect": false },
    { "choiceText": "がっしょう", "isCorrect": false }
  ]
}
```

### Example 4: Response with Explanation
```bash
POST /api/questions/generate-ai
{
  "level": "N3",
  "contentName": "Grammar",
  "description": "Chọn ngữ pháp phù hợp với câu"
}
```
Expected Response:
```json
{
  "questionText": "次の文の（　）に入る最も適切な語を選びなさい。\n彼は毎日一生懸命（　）勉強している。",
  "explanation": "Đáp án 'に' đúng vì sau trạng từ '一生懸命' cần có trợ từ 'に' để biểu thị cách thức thực hiện hành động.",
  "choices": [
    { "choiceText": "に", "isCorrect": true },
    { "choiceText": "で", "isCorrect": false },
    { "choiceText": "を", "isCorrect": false },
    { "choiceText": "が", "isCorrect": false }
  ]
}
```

---

## Technical Implementation

### Files Modified
1. `GeneratedQuestionResponseDto.cs` — add `Explanation`
2. `QuestionService.cs` — return `Explanation` from AI result
3. `GeminiService.cs` — prompt updated to request Vietnamese explanation

### Existing Infrastructure Used
- `AIGeneratedQuestionResult.Explanation`
- `GeminiQuestionData.explanation`
- `Question.explanation` entity field

---

## Configuration Integration (Fixed)

### Prior Issues
- `GeminiConfiguration` previously registered with a non-validated pattern in `Persistence/DependencyInjection.cs`
- `LogEnvironmentVariables` only logs configurations registered via `AddValidatedConfiguration`

### Fix Applied
```csharp
// Program.cs - ADDED:
builder.Services.AddValidatedConfiguration<GeminiConfiguration>(config);

// Persistence/DependencyInjection.cs - REMOVED:
// services.Configure<GeminiConfiguration>(configuration.GetSection(GeminiConfiguration.SectionName));
```

### Result
- Validation: Gemini configuration validated at startup
- Debug Log: Appears in `LogEnvironmentVariables` when `Api__ShowConfigurationStatus=true`
- Consistency: All configurations now use the validated pattern

---

## Configuration Debugging

### 1) Set environment variable (.env)
```env
Api__ShowConfigurationStatus="true"
```

### 2) Restart application
```bash
dotnet run
```

### 3) Check console output
```
Environment variables loaded
=== ENVIRONMENT VARIABLES DEBUG ===
[GeminiAI Configuration]
ApiKey: your-gemini-api-key-here
...
```

---

## Build and Status

- Build: 0 errors, 19 warnings (pre-existing)
- Configuration: Proper validation + debug logging
- API: Enhanced with description field and Vietnamese explanation
- AI Prompts: Specific guidelines for each question type
- Documentation: Complete usage examples

---

## Next Steps

1. Test API endpoint with various question types
2. Verify Vietnamese explanation quality
3. Consider adding explanation validation
4. Update frontend to display explanation
