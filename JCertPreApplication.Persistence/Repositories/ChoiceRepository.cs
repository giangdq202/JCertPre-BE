using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class ChoiceRepository : GenericRepository<Choice>, IChoiceRepository
    {
        public ChoiceRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Choice>> GetByQuestionIdAsync(Guid questionId)
        {
            return await _dbSet
                .Where(c => c.questionId == questionId)
                .ToListAsync();
        }

        public async Task<Choice?> GetByIdAsync(Guid choiceId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.choiceId == choiceId);
        }

        public async Task<Choice> AddAsync(Guid questionId, Choice choice)
        {
            choice.questionId = questionId;
            await _dbSet.AddAsync(choice);
            await _context.SaveChangesAsync();
            return choice;
        }

        public async Task UpdateListAsync(Guid questionId, IEnumerable<Choice> choices)
        {
            var existingChoices = await _dbSet.Where(c => c.questionId == questionId).ToListAsync();
            var existingDict = existingChoices.ToDictionary(c => c.choiceId);

            foreach (var choice in choices)
            {
                if (existingDict.TryGetValue(choice.choiceId, out var existing))
                {
                    existing.choiceText = choice.choiceText;
                    existing.isCorrect = choice.isCorrect;
                    _dbSet.Update(existing);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid questionId, Guid choiceId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(c => c.choiceId == choiceId && c.questionId == questionId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}