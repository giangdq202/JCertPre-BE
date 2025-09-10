using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class ChoiceServiceFixture
{
    public ChoiceService ChoiceService { get; }
    public Mock<IChoiceRepository> MockChoiceRepository { get; }
    public Mock<IQuestionRepository> MockQuestionRepository { get; } 

    public ChoiceServiceFixture()
    {
        MockChoiceRepository = new Mock<IChoiceRepository>();
        MockQuestionRepository = new Mock<IQuestionRepository>();

        ChoiceService = new ChoiceService(MockChoiceRepository.Object, MockQuestionRepository.Object);
    }

    public static List<Choice> CreateChoicesForQuestion(Guid questionId, int count, int correctIndex = 0)
    {
        var choices = new List<Choice>();
        for (int i = 0; i < count; i++)
        {
            choices.Add(ChoiceBuilder.Create()
                .WithQuestionId(questionId)
                .WithText($"Choice {i + 1}")
                .WithIsCorrect(i == correctIndex)
                .Build());
        }
        return choices;
    }

    public static ChoiceCreateDto ValidCreateDto(string content = "Test choice", bool isCorrect = false)
    {
        return new ChoiceCreateDto
        {
            Content = content,
            IsCorrect = isCorrect
        };
    }

    public static ChoiceUpdateDto ValidUpdateDto(string? content = null, bool? isCorrect = null)
    {
        return new ChoiceUpdateDto
        {
            Content = content,
            IsCorrect = isCorrect
        };
    }

    public static ChoiceReadDto CreateChoiceDto(Choice choice)
    {
        return new ChoiceReadDto
        {
            ChoiceId = choice.choiceId,
            QuestionId = choice.questionId,
            Content = choice.choiceText,
            IsCorrect = choice.isCorrect
        };
    }

    public static List<ChoiceCreateDto> CreateMultipleChoiceDtos(int count)
    {
        var dtos = new List<ChoiceCreateDto>();
        for (int i = 0; i < count; i++)
        {
            dtos.Add(ValidCreateDto($"Choice {i + 1}", i == 0));
        }
        return dtos;
    }
}
