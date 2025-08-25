using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class QuestionAttachmentBuilder
    {
        private QuestionAttachment _attachment;

        public QuestionAttachmentBuilder()
        {
            _attachment = new QuestionAttachment
            {
                attachmentId = Guid.NewGuid(),
                questionId = Guid.NewGuid(),
                mediaUrl = "https://example.com/test-media.mp3",
                mediaType = "audio"
            };
        }

        public static QuestionAttachmentBuilder Create() => new QuestionAttachmentBuilder();

        public QuestionAttachmentBuilder WithId(Guid id)
        {
            _attachment.attachmentId = id;
            return this;
        }

        public QuestionAttachmentBuilder WithQuestionId(Guid questionId)
        {
            _attachment.questionId = questionId;
            return this;
        }

        public QuestionAttachmentBuilder WithMediaUrl(string mediaUrl)
        {
            _attachment.mediaUrl = mediaUrl;
            return this;
        }

        public QuestionAttachmentBuilder WithMediaType(string mediaType)
        {
            _attachment.mediaType = mediaType;
            return this;
        }

        public QuestionAttachmentBuilder AsAudio()
        {
            _attachment.mediaType = "audio";
            return this;
        }

        public QuestionAttachmentBuilder AsImage()
        {
            _attachment.mediaType = "image";
            return this;
        }

        public QuestionAttachmentBuilder WithQuestion(Question question)
        {
            _attachment.Question = question;
            _attachment.questionId = question.questionId;
            return this;
        }

        public QuestionAttachment Build() => _attachment;
    }
}
