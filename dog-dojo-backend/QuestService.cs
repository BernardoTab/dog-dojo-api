using dog_dojo_backend.Entities;
using dog_dojo_backend.Interfaces;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace dog_dojo_backend
{
    public class QuestService : IQuestService
    {
        private readonly string _questsFile = "quest-list.json";
        private IQuestRepository _questRepository;
        private readonly ILogger<QuestService> _logger;
        private readonly Random _random = new();

        public QuestService(
            IQuestRepository questRepository, 
            ILogger<QuestService> logger)
        {
            _questRepository = questRepository;
            _logger = logger;
        }

        public async Task<CurrentQuest?> GetCurrentQuestAsync()
        {
            return await _questRepository.GetCurrentQuestAsync();
        }

        public async Task<bool> CheckIfNewQuestNeededAsync()
        {
            var currentQuest = await _questRepository.GetCurrentQuestAsync();
            if (currentQuest == null) return true;

            if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Monday) return true;

            return false;
        }

        public async Task<bool> RefreshQuestAsync()
        {
            try
            {
                var quests = await GetSideQuestsFromJsonAsync();
                var completedQuests = await _questRepository.GetCompletedQuestsAsync();

                if (quests == null)
                {
                    _logger.LogError("Could not read from quests file.");
                    return false;
                }

                // Filter available quests
                var availableQuests = quests
                    .Where(q => !completedQuests.Select(q => q.QuestId).Contains(q.Id))
                    .ToList();

                if (availableQuests.Count == 0)
                {
                    _logger.LogError("All quests used. Add more quests to continue.");
                    return false;
                }

                var chosenSideQuest = availableQuests[_random.Next(availableQuests.Count)];

                //complete current quest
                var existingCurrentQuest = await _questRepository.GetCurrentQuestAsync();
                if (existingCurrentQuest != null)
                {
                    await _questRepository.AddCompletedQuestAsync(new CompletedQuest
                    {
                        CompletedDate = DateTime.UtcNow,
                        QuestId = existingCurrentQuest.QuestId,
                        StartDate = existingCurrentQuest.StartDate
                    });
                }

                //add new quest
                await _questRepository.UpdateCurrentQuestAsync(new CurrentQuest
                {
                    QuestId = chosenSideQuest.Id,
                    Title = chosenSideQuest.Title,
                    Description = chosenSideQuest.Description,
                    StartDate = DateTime.UtcNow
                });

                _logger.LogInformation($"Selected quest of the week {chosenSideQuest.Id}: {chosenSideQuest.Title}");
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} occurred");
                return false;
            }
            
        }

        private async Task<ICollection<SideQuest>?> GetSideQuestsFromJsonAsync()
        {
            if (!File.Exists(_questsFile))
                throw new FileNotFoundException("Side quests file not found");

            var json = await File.ReadAllTextAsync(_questsFile, Encoding.UTF8);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var quests = JsonSerializer.Deserialize<List<SideQuest>>(json, options);

            return quests;
        }
    }
}
