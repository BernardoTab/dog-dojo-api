using dog_dojo_backend.Entities;

namespace dog_dojo_backend.Interfaces
{
    public interface IQuestService
    {
        Task<bool> CheckIfNewQuestNeededAsync();
        Task<bool> RefreshQuestAsync();
        Task<CurrentQuest?> GetCurrentQuestAsync();
    }
}
