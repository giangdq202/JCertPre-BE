using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Persistence.Models;

namespace JCertPreApplication.Persistence.Services
{
    public class GeminiService : IAIIntegration
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly GeminiConfiguration _config;
        private readonly SemaphoreSlim _rateLimitSemaphore;

        public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, IOptions<GeminiConfiguration> config)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
            
            // Rate limiting: 10 RPM = 1 request per 6 seconds
            _rateLimitSemaphore = new SemaphoreSlim(1, 1);
            
            // Configure HttpClient
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _config.ApiKey);
        }

        public async Task<AIGeneratedQuestionResult> GenerateQuestionAsync(string level, string contentName, string description)
        {
            try
            {
                _logger.LogInformation("Generating JLPT question: {Level} {ContentName}", level, contentName);

                // Rate limiting
                await _rateLimitSemaphore.WaitAsync();
                try
                {
                    var prompt = BuildPrompt(level, contentName, description);
                    var response = await CallGeminiApiAsync(prompt);
                    return ProcessGeminiResponse(response);
                }
                finally
                {
                    // Release rate limit after 6 seconds
                    _ = Task.Delay(6000).ContinueWith(_ => _rateLimitSemaphore.Release());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating question with AI");
                return new AIGeneratedQuestionResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<string> GenerateExplanationAsync(string questionText, List<AIGeneratedChoice> choices)
        {
            try
            {
                _logger.LogInformation("Generating explanation for question");

                // Rate limiting
                await _rateLimitSemaphore.WaitAsync();
                try
                {
                    var prompt = BuildExplanationPrompt(questionText, choices);
                    var response = await CallGeminiApiForExplanationAsync(prompt);
                    return ProcessExplanationResponse(response);
                }
                finally
                {
                    // Release rate limit after 6 seconds
                    _ = Task.Delay(6000).ContinueWith(_ => _rateLimitSemaphore.Release());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating explanation with AI");
                return "Không thể tạo giải thích lúc này. Vui lòng thử lại sau.";
            }
        }

        private string BuildPrompt(string level, string contentName, string description)
        {
            return $@"
You are an expert in Japanese language education and JLPT (Japanese Language Proficiency Test) content creation.
Your task is to generate a high-quality multiple-choice question based on the following criteria.

**Context:**
- JLPT Level: ""{level}"" (N5, N4, N3, N2, or N1)
- Content Type: ""{contentName}""
- Question Type: ""{description}""

**Requirements:**
- The question must be appropriate for JLPT {level} level.
- The question must follow the type: ""{description}""
- The question must have exactly 4 choices.
- Exactly 1 of the choices must be correct (single correct answer format).
- All content must be accurate according to standard Japanese grammar and usage.
- Use appropriate Japanese characters (hiragana, katakana, kanji) suitable for the specified level.
- Provide a clear explanation in Vietnamese explaining why the correct answer is right.

**Content-Specific Guidelines:**
{GetContentSpecificGuidelines(contentName)}

**Question Type Guidelines:**
{GetQuestionTypeGuidelines(description)}

**Output Format:**
Please return ONLY a single, raw JSON object with the following structure. Do not include any explanations, comments, or markdown formatting:

{{
  ""questionText"": ""[Your question text here]"",
  ""explanation"": ""[Giải thích ngắn gọn tại sao đáp án này đúng bằng tiếng Việt]"",
  ""choices"": [
    {{
      ""content"": ""[Choice 1 text]"",
      ""isCorrect"": true
    }},
    {{
      ""content"": ""[Choice 2 text]"",
      ""isCorrect"": false
    }},
    {{
      ""content"": ""[Choice 3 text]"",
      ""isCorrect"": false
    }},
    {{
      ""content"": ""[Choice 4 text]"",
      ""isCorrect"": false
    }}
  ]
}}
";
        }

        private string GetContentSpecificGuidelines(string contentName)
        {
            return contentName switch
            {
                "Kanji" => @"- Focus on kanji readings, meanings, or stroke order
- Questions can test on'yomi, kun'yomi, or compound readings
- Include context sentences when appropriate
- Choices should be related kanji or readings",
                
                "Vocabulary" => @"- Focus on word meanings, usage, or synonyms/antonyms
- Test vocabulary appropriate for the JLPT level
- Include context sentences to show proper usage
- Choices should be semantically related words",
                
                "Grammar" => @"- Focus on grammar patterns, particles, or sentence structure
- Test grammar points appropriate for the JLPT level
- Provide context to show proper grammatical usage
- Include common grammar mistakes as incorrect choices",
                
                "Reading" => @"- Create a short passage or sentence for comprehension
- Ask about main ideas, details, or inference
- Include reading strategies like context clues
- Test reading skills appropriate for the level",
                
                _ => "Follow standard JLPT question format."
            };
        }

        private string GetQuestionTypeGuidelines(string description)
        {
            return description switch
            {
                "Đọc chữ Hán" => @"- Create a question testing kanji reading (pronunciation)
- Show a kanji or compound and ask for the correct reading
- Include hiragana options for different readings
- Focus on common readings for the JLPT level",

                "Nhớ chữ Hán" => @"- Create a question testing kanji meaning or recognition
- Show a meaning/context and ask for the correct kanji
- Include similar-looking kanji as distractors
- Test kanji knowledge appropriate for the level",

                "Chọn từ phù hợp với câu" => @"- Create a sentence with a blank to fill in
- Provide vocabulary options that fit grammatically but only one fits contextually
- Test understanding of word usage and meaning
- Include similar words as distractors",

                "Tìm câu có cách diễn đạt giống" => @"- Present a sentence and ask for the equivalent expression
- Test understanding of synonymous expressions
- Include options with similar grammar but different meanings
- Focus on natural Japanese expressions",

                "Chọn ngữ pháp phù hợp với câu" => @"- Create a sentence with a grammar point to test
- Provide grammar pattern options
- Test specific grammar structures for the JLPT level
- Include common grammar mistakes as wrong options",

                "Sắp xếp câu" => @"- Provide scrambled sentence parts to arrange
- Test understanding of Japanese word order and sentence structure
- Include grammar particles in the arrangement
- Focus on natural sentence flow",

                "Tìm đáp án đúng để hoàn thành đoạn văn" => @"- Create a short passage with missing parts
- Test reading comprehension and context understanding
- Provide options that fit grammatically but only one fits contextually
- Test coherence and flow of ideas",

                "Đoạn văn ngắn" => @"- Create a short passage (2-3 sentences) with a comprehension question
- Ask about main ideas, details, or specific information
- Test basic reading comprehension skills
- Include questions about explicit information",

                "Trung văn" => @"- Create a medium-length passage with comprehension questions
- Test deeper understanding of content and context
- Ask about inference, main ideas, or author's intent
- Include more complex vocabulary and grammar",

                "Tìm kiếm thông tin" => @"- Create a passage where students need to locate specific information
- Test ability to scan and find details quickly
- Ask questions that require identifying specific facts or data
- Focus on information retrieval skills",

                "Hiểu đề bài" => @"- Create a listening comprehension scenario about understanding instructions
- Test ability to comprehend task requirements or directions
- Include questions about what action should be taken
- Focus on practical listening situations",

                "Hiểu điểm chính" => @"- Create a listening scenario testing main idea comprehension
- Ask about the central theme or key message
- Test ability to distinguish main points from details
- Focus on overall understanding of content",

                "Diễn đạt bằng lời nói" => @"- Create a scenario testing understanding of spoken expressions
- Test comprehension of natural speech patterns
- Include questions about tone, intention, or implied meaning
- Focus on conversational Japanese",

                "Phản hồi tức thời" => @"- Create a dialogue scenario requiring immediate response comprehension
- Test understanding of quick exchanges or reactions
- Include questions about appropriate responses
- Focus on real-time communication skills",

                _ => "Follow the general question format appropriate for the content type and JLPT level."
            };
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: _config.MaxRetries,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {Delay}ms for Gemini API call", retryCount, timespan.TotalMilliseconds);
                    });

            return await retryPolicy.ExecuteAsync(async () =>
            {
                var requestBody = new GeminiRequest
                {
                    contents = new[]
                    {
                        new GeminiContent
                        {
                            parts = new[] { new GeminiPart { text = prompt } }
                        }
                    },
                    generationConfig = new GeminiGenerationConfig
                    {
                        response_mime_type = "application/json",
                        response_schema = GetResponseSchema()
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var url = $"{_config.BaseUrl}/models/{_config.Model}:generateContent";

                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Gemini API error: {response.StatusCode}");
                }

                return await response.Content.ReadAsStringAsync();
            });
        }

        private object GetResponseSchema()
        {
            return new
            {
                type = "object",
                properties = new
                {
                    questionText = new { type = "string" },
                    explanation = new { type = "string" },
                    choices = new
                    {
                        type = "array",
                        items = new
                        {
                            type = "object",
                            properties = new
                            {
                                content = new { type = "string" },
                                isCorrect = new { type = "boolean" }
                            },
                            required = new[] { "content", "isCorrect" }
                        }
                    }
                },
                required = new[] { "questionText", "explanation", "choices" }
            };
        }

        private AIGeneratedQuestionResult ProcessGeminiResponse(string responseContent)
        {
            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var generatedText = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text;
                
                if (string.IsNullOrEmpty(generatedText))
                {
                    _logger.LogWarning("Empty response from Gemini API");
                    return new AIGeneratedQuestionResult
                    {
                        IsValid = false,
                        ErrorMessage = "Empty response from AI service"
                    };
                }

                var questionData = JsonSerializer.Deserialize<GeminiQuestionData>(generatedText, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (questionData == null)
                {
                    return new AIGeneratedQuestionResult
                    {
                        IsValid = false,
                        ErrorMessage = "Failed to parse AI response"
                    };
                }

                // Validate response
                if (string.IsNullOrEmpty(questionData.questionText) || 
                    questionData.choices?.Length != 4)
                {
                    return new AIGeneratedQuestionResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid question format from AI"
                    };
                }

                var correctCount = questionData.choices.Count(c => c.isCorrect);
                if (correctCount != 1)
                {
                    return new AIGeneratedQuestionResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Question must have exactly 1 correct answer, got {correctCount}"
                    };
                }

                return new AIGeneratedQuestionResult
                {
                    QuestionText = questionData.questionText,
                    Explanation = questionData.explanation ?? string.Empty,
                    Choices = questionData.choices.Select(c => new AIGeneratedChoice
                    {
                        Content = c.content,
                        IsCorrect = c.isCorrect
                    }).ToList(),
                    IsValid = true
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse Gemini response");
                return new AIGeneratedQuestionResult
                {
                    IsValid = false,
                    ErrorMessage = "Failed to parse AI response"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini response");
                return new AIGeneratedQuestionResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private string BuildExplanationPrompt(string questionText, List<AIGeneratedChoice> choices)
        {
            var correctChoice = choices.FirstOrDefault(c => c.IsCorrect);

            var choicesText = new StringBuilder();
            for (int i = 0; i < choices.Count; i++)
            {
                choicesText.AppendLine($"- {choices[i].Content}");
            }

            return $@"
You are an AI assistant for a Japanese learning platform. Your task is to provide a clear and concise explanation for a multiple-choice question.

Analyze the following question:

**Question:**
{questionText}

**Choices:**
{choicesText}

**The correct answer is:**
{correctChoice?.Content}

**Your Task:**
Write a detailed but easy-to-understand explanation in **Vietnamese**. Explain why the provided answer is correct and why the other options are incorrect if relevant. The tone should be educational and encouraging for a student learning Japanese.

**Requirements:**
- Write ONLY the explanation text in Vietnamese
- Keep it concise but informative (2-4 sentences)
- Use simple, clear language
- Focus on the learning aspect
- Do not include any JSON formatting or additional text
- Do not repeat the question or choices in your response

Please provide only the explanation text:
";
        }

        private async Task<string> CallGeminiApiForExplanationAsync(string prompt)
        {
            try
            {
                var requestBody = new GeminiRequest
                {
                    contents = new[]
                    {
                        new GeminiContent
                        {
                            parts = new[] { new GeminiPart { text = prompt } }
                        }
                    },
                    generationConfig = new GeminiGenerationConfig()
                };

                var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var url = $"{_config.BaseUrl}/models/{_config.Model}:generateContent";

                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Gemini API error: {response.StatusCode}");
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API for explanation");
                throw;
            }
        }

        private string ProcessExplanationResponse(string responseContent)
        {
            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var generatedText = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text;
                
                if (string.IsNullOrEmpty(generatedText))
                {
                    _logger.LogWarning("Empty response from Gemini API");
                    return "Không thể tạo giải thích lúc này. Vui lòng thử lại sau.";
                }

                // Clean up the response - remove any extra formatting
                var cleanedText = generatedText.Trim();
                
                return cleanedText;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse Gemini response");
                return "Không thể tạo giải thích lúc này. Vui lòng thử lại sau.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Gemini response");
                return "Không thể tạo giải thích lúc này. Vui lòng thử lại sau.";
            }
        }

        public void Dispose()
        {
            _rateLimitSemaphore?.Dispose();
        }
    }
}
