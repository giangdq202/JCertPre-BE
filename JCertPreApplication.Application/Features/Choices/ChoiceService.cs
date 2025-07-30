using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Choice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Choices
{
    /// <summary>
    /// Service for handling business logic related to Choice entities.
    /// Implements exception handling and follows Clean Architecture best practices.
    /// </summary>
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _choiceRepo;

        public ChoiceService(IChoiceRepository choiceRepo)
        {
            _choiceRepo = choiceRepo ?? throw new ArgumentNullException(nameof(choiceRepo));
        }

        /// <summary>
        /// Retrieves all choices for a question.
        /// </summary>
        public async Task<IEnumerable<ChoiceReadDto>> GetByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var choices = await _choiceRepo.GetAllAsync(c => c.questionId == questionId);
                return choices.Select(MapToChoiceReadDto);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_SERVICE_ERROR", $"An error occurred while retrieving choices: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new choice for a question.
        /// </summary>
        public async Task<ChoiceReadDto> CreateAsync(Guid questionId, ChoiceCreateDto dto)
        {
            try
            {
                var choice = new Choice
                {
                    choiceId = Guid.NewGuid(),
                    questionId = questionId,
                    choiceText = dto.Content,
                    isCorrect = dto.IsCorrect
                };
                var created = await _choiceRepo.InsertAsync(choice);
                await _choiceRepo.SaveChangesAsync();
                return MapToChoiceReadDto(created);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_CREATE_ERROR", $"An error occurred while creating choice: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a choice by its ID and question ID.
        /// /// </summary>
        public async Task UpdateAsync(Guid choiceId, ChoiceUpdateDto dto)
        {
            try
            {
                var choice = await _choiceRepo.GetByIdAsync(choiceId);
                if (choice == null)
                    throw ApiException.NotFound("Choice", choiceId);

                if (dto.Content != null)
                    choice.choiceText = dto.Content;
                if (dto.IsCorrect.HasValue)
                    choice.isCorrect = dto.IsCorrect.Value;

                await _choiceRepo.UpdateAsync(choice);
                await _choiceRepo.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_UPDATE_ERROR", $"An error occurred while updating choice: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a choice by its ID and question ID.
        /// </summary>
        public async Task DeleteAsync(Guid choiceId)
        {
            try
            {
                var choice = await _choiceRepo.GetByIdAsync(choiceId);
                if (choice == null)
                    throw ApiException.NotFound("Choice", choiceId);

                await _choiceRepo.DeleteAsync(choice);
                await _choiceRepo.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_DELETE_ERROR", $"An error occurred while deleting choice: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps a Choice entity to a ChoiceReadDto.
        /// </summary>
        private static ChoiceReadDto MapToChoiceReadDto(Choice c)
        {
            return new ChoiceReadDto
            {
                ChoiceId = c.choiceId,
                QuestionId = c.questionId,
                Content = c.choiceText,
                IsCorrect = c.isCorrect
            };
        }
    }
}