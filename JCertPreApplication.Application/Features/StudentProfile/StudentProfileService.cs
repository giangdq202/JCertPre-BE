using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Application.Features.StudentProfile
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly IStudentProfileRepository _studentProfileRepository;
        public StudentProfileService(IStudentProfileRepository studentProfileRepository)
        {
            _studentProfileRepository = studentProfileRepository ?? throw new ArgumentNullException(nameof(studentProfileRepository));
        }
        
        public async Task<StudentProfileDto> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            try
            {
                var profileTask = _studentProfileRepository.CreateStudentProfileAsync(userId, currentLevel, learningGoals);
                var profile = await profileTask;
                if (profile == null)
                    throw ApiException.InternalServerError("STUDENT_PROFILE_CREATE_ERROR", "Student profile creation failed.");

                return MapToStudentProfileDto(profile);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDENT_PROFILE_SERVICE_ERROR", $"An error occurred while creating student profile: {ex.Message}");
            }
        }

        public async Task<bool> DeleteStudentProfileAsync(Guid userId)
        {
            try
            {
                // Check if profile exists before deleting
                var existingProfileTask = _studentProfileRepository.ReadStudentProfileAsync(userId);
                var existingProfile = await existingProfileTask;
                if (existingProfile == null)
                    throw ApiException.NotFound("StudentProfile", userId);

                return await _studentProfileRepository.DeleteStudentProfileAsync(userId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDENT_PROFILE_DELETE_ERROR", $"An error occurred while deleting student profile: {ex.Message}");
            }
        }

        public async Task<StudentProfileDto> GetStudentProfileAsync(Guid userId)
        {
            try
            {
                var profileTask = _studentProfileRepository.ReadStudentProfileAsync(userId);
                var profile = await profileTask;
                if (profile == null)
                    throw ApiException.NotFound("StudentProfile", userId);

                return MapToStudentProfileDto(profile);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDENT_PROFILE_SERVICE_ERROR", $"An error occurred while retrieving student profile: {ex.Message}");
            }
        }

        public async Task<StudentProfileDto> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            try
            {
                // Check if profile exists before updating
                var existingProfileTask = _studentProfileRepository.ReadStudentProfileAsync(userId);
                var existingProfile = await existingProfileTask;
                if (existingProfile == null)
                    throw ApiException.NotFound("StudentProfile", userId);

                var profileTask = _studentProfileRepository.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);
                var profile = await profileTask;
                return MapToStudentProfileDto(profile);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDENT_PROFILE_UPDATE_ERROR", $"An error occurred while updating student profile: {ex.Message}");
            }
        }

        private static StudentProfileDto MapToStudentProfileDto(Domain.Entities.StudentProfile profile)
        {
            return new StudentProfileDto
            {
                UserId = profile.userId,
                CurrentLevel = profile.currentLevel,
                LearningGoals = profile.learningGoals
            };
        }
    }
}
