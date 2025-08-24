using JCertPreApplication.Domain.Entities;
using System;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class ChoiceBuilder
{
    private Choice _choice;

    public ChoiceBuilder()
    {
        _choice = new Choice
        {
            choiceId = Guid.NewGuid(),
            questionId = Guid.NewGuid(),
            choiceText = "Test choice content",
            isCorrect = false
        };
    }

    public static ChoiceBuilder Create() => new ChoiceBuilder();

    public ChoiceBuilder WithId(Guid id)
    {
        _choice.choiceId = id;
        return this;
    }

    public ChoiceBuilder WithQuestionId(Guid questionId)
    {
        _choice.questionId = questionId;
        return this;
    }

    public ChoiceBuilder WithText(string text)
    {
        _choice.choiceText = text;
        return this;
    }

    public ChoiceBuilder WithIsCorrect(bool isCorrect)
    {
        _choice.isCorrect = isCorrect;
        return this;
    }

    public ChoiceBuilder AsCorrectAnswer()
    {
        _choice.isCorrect = true;
        return this;
    }

    public ChoiceBuilder AsWrongAnswer()
    {
        _choice.isCorrect = false;
        return this;
    }

    public Choice Build() => _choice;
}