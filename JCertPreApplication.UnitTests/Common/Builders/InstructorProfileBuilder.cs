using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class InstructorProfileBuilder
    {
        private InstructorProfile _instructorProfile;

        public InstructorProfileBuilder()
        {
            _instructorProfile = new InstructorProfile
            {
                userId = Guid.NewGuid(),
                introduction = "Experienced Japanese language instructor with 5+ years of teaching experience.",
                experience = "5+ years teaching Japanese to international students",
                teachingStyle = "Interactive and communicative approach"
            };
        }

        public static InstructorProfileBuilder Create() => new InstructorProfileBuilder();

        public InstructorProfileBuilder WithId(Guid id)
        {
            _instructorProfile.userId = id;
            return this;
        }

        public InstructorProfileBuilder WithIntroduction(string introduction)
        {
            _instructorProfile.introduction = introduction;
            return this;
        }

        public InstructorProfileBuilder WithExperience(string experience)
        {
            _instructorProfile.experience = experience;
            return this;
        }

        public InstructorProfileBuilder WithTeachingStyle(string teachingStyle)
        {
            _instructorProfile.teachingStyle = teachingStyle;
            return this;
        }

        public InstructorProfileBuilder WithEmptyExperience()
        {
            _instructorProfile.experience = null;
            return this;
        }

        public InstructorProfileBuilder WithEmptyTeachingStyle()
        {
            _instructorProfile.teachingStyle = null;
            return this;
        }

        public InstructorProfileBuilder WithMinimalData()
        {
            _instructorProfile.introduction = "Basic introduction";
            _instructorProfile.experience = null;
            _instructorProfile.teachingStyle = null;
            return this;
        }

        public InstructorProfile Build() => _instructorProfile;
    }
}
