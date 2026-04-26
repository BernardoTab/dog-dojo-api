using dog_dojo_backend.Entities;

namespace dog_dojo_backend.Interfaces
{
    public interface IQuestRepository
    {
        Task<CurrentQuest?> GetCurrentQuestAsync();
        Task<ICollection<CompletedQuest>> GetCompletedQuestsAsync();
        Task UpdateCurrentQuestAsync(CurrentQuest quest);
        Task AddCompletedQuestAsync(CompletedQuest quest);
    }
}
