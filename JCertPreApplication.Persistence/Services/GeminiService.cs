using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
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

        public async Task<AIGeneratedQuestionResult> GenerateQuestionAsync(string level, string contentName)
        {
            try
            {
                _logger.LogInformation("Generating JLPT question: {Level} {ContentName}", level, contentName);

                // Rate limiting
                await _rateLimitSemaphore.WaitAsync();
                try
                {
                    var prompt = BuildPrompt(level, contentName);
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

        private string BuildPrompt(string level, string contentName)
        {
            return $@"
You are an expert in Japanese language education and JLPT (Japanese Language Proficiency Test) content creation.
Your task is to generate a high-quality multiple-choice question based on the following criteria.

**Context:**
- JLPT Level: ""{level}"" (N5, N4, N3, N2, or N1)
- Content Type: ""{contentName}""

**Requirements:**
- The question must be appropriate for JLPT {level} level.
- The question must have exactly 4 choices.
- Exactly 1 of the choices must be correct (single correct answer format).
- All content must be accurate according to standard Japanese grammar and usage.
- Use appropriate Japanese characters (hiragana, katakana, kanji) suitable for the specified level.

**Content-Specific Guidelines:**
{GetContentSpecificGuidelines(contentName)}

**Output Format:**
Please return ONLY a single, raw JSON object with the following structure. Do not include any explanations, comments, or markdown formatting:

{{
  ""questionText"": ""[Your question text here]"",
  ""explanation"": ""[Brief explanation of the correct answer]"",
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

        public void Dispose()
        {
            _rateLimitSemaphore?.Dispose();
        }
    }
}
