using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Application.Features.InstructorProfile
{
    public class InstructorProfileService : IInstructorProfileService
    {
        private readonly IInstructorProfileRepository _instructorProfileRepository;
        public InstructorProfileService(IInstructorProfileRepository instructorProfileRepository)
        {
            _instructorProfileRepository = instructorProfileRepository ?? throw new ArgumentNullException(nameof(instructorProfileRepository));
        }
        
        public async Task<InstructorProfileDto> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            try
            {
                var profileTask = _instructorProfileRepository.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
                var profile = await profileTask;
                if (profile == null)
                    throw ApiException.InternalServerError("INSTRUCTOR_PROFILE_CREATE_ERROR", "Instructor profile creation failed.");

                return MapToInstructorProfileDto(profile);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("INSTRUCTOR_PROFILE_SERVICE_ERROR", $"An error occurred while creating instructor profile: {ex.Message}");
            }
        }

        public async Task<bool> DeleteInstructorProfileAsync(Guid userId)
        {
            try
            {
                // Check if profile exists before deleting
                var existingProfileTask = _instructorProfileRepository.ReadInstructorProfileAsync(userId);
                var existingProfile = await existingProfileTask;
                if (existingProfile == null)
                    throw ApiException.NotFound("InstructorProfile", userId);

                return await _instructorProfileRepository.DeleteInstructorProfileAsync(userId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("INSTRUCTOR_PROFILE_DELETE_ERROR", $"An error occurred while deleting instructor profile: {ex.Message}");
            }
        }

        public async Task<InstructorProfileDto> GetInstructorProfileAsync(Guid userId)
        {
            try
            {
                var profileTask = _instructorProfileRepository.ReadInstructorProfileAsync(userId);
                var profile = await profileTask;
                if (profile == null)
                    throw ApiException.NotFound("InstructorProfile", userId);

                return MapToInstructorProfileDto(profile);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("INSTRUCTOR_PROFILE_SERVICE_ERROR", $"An error occurred while retrieving instructor profile: {ex.Message}");
            }
        }

        public async Task<InstructorProfileDto> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            try
            {
                // Check if profile exists before updating
                var existingProfileTask = _instructorProfileRepository.ReadInstructorProfileAsync(userId);
                var existingProfile = await existingProfileTask;
                if (existingProfile == null)
                    throw ApiException.NotFound("InstructorProfile", userId);

                var profileTask = _instructorProfileRepository.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
                var profile = await profileTask;
                return MapToInstructorProfileDto(profile);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("INSTRUCTOR_PROFILE_UPDATE_ERROR", $"An error occurred while updating instructor profile: {ex.Message}");
            }
        }
        
        private static InstructorProfileDto MapToInstructorProfileDto(Domain.Entities.InstructorProfile profile)
        {
            return new InstructorProfileDto
            {
                UserId = profile.userId,
                Introduction = profile.introduction,
                Experience = profile.experience,
                TeachingStyle = profile.teachingStyle
            };
        }
    }
}
