using dog_dojo_backend.Entities;
using dog_dojo_backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace dog_dojo_backend
{
    public class QuestRepository : IQuestRepository
    {
        protected readonly DogDojoDbContext _context;

        public QuestRepository(DogDojoDbContext context)
        {
            _context = context;
        }

        public async Task<CurrentQuest?> GetCurrentQuestAsync()
        {
            return await _context.CurrentQuests.FirstOrDefaultAsync();
        }

        public async Task UpdateCurrentQuestAsync(CurrentQuest quest)
        {
            var currentQuest = await _context.CurrentQuests.FirstOrDefaultAsync();
            if (currentQuest != null)
            {
                currentQuest.QuestId = quest.Id;
                currentQuest.Title = quest.Title;
                currentQuest.Description = quest.Description;
                _context.Update(currentQuest);
            } else
            {
                await _context.CurrentQuests.AddAsync(quest);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CompletedQuest>> GetCompletedQuestsAsync()
        {
            return await _context.CompletedQuests.ToListAsync();
        }

        public async Task AddCompletedQuestAsync(CompletedQuest quest)
        {
            await _context.CompletedQuests.AddAsync(quest);
            await _context.SaveChangesAsync();
        }
    }
}
