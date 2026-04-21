namespace dog_dojo_backend
{
    using System.Text;
    using System.Text.Json;

    public class WeeklyQuestWorker : BackgroundService
    {
        private readonly ILogger<WeeklyQuestWorker> _logger;
        private readonly string _questsFile = "quest-list.json";
        private readonly string _historyFile = "chosen_quests.txt";
        private readonly string _currentQuestFile = "current_quest.json";
        private readonly Random _random = new();
        public SideQuest _chosenSideQuest;

        public WeeklyQuestWorker(ILogger<WeeklyQuestWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WeeklyQuestWorker started");
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                bool isScheduledTime =
                    now.DayOfWeek == DayOfWeek.Monday &&
                    now.Hour == 0 &&
                    now.Minute == 0;

                bool isCurrentQuestMissing =
                    !File.Exists(_currentQuestFile) ||
                    new FileInfo(_currentQuestFile).Length == 0;

                if (isScheduledTime || isCurrentQuestMissing)
                {
                    try
                    {
                        await RunTaskSelection();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error selecting quest");
                    }

                    // Prevent multiple executions within the same minute
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                else
                {
                    // Check every 20 seconds
                    _logger.LogInformation("Working, waiting for Monday zzz");
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                }
            }
        }

        private async Task RunTaskSelection()
        {
            if (!File.Exists(_questsFile))
                throw new FileNotFoundException("Side quests file not found");

            var json = await File.ReadAllTextAsync(_questsFile, Encoding.UTF8); 
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var quests = JsonSerializer.Deserialize<List<SideQuest>>(json,options);

            if (quests == null || quests.Count == 0)
                throw new Exception("No quests available");

            // Load used IDs
            var usedIds = new HashSet<int>();

            if (File.Exists(_historyFile))
            {
                var lines = await File.ReadAllLinesAsync(_historyFile);

                foreach (var line in lines)
                {
                    var parts = line.Split("Chosen Quest ID:");
                    if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int id))
                    {
                        usedIds.Add(id);
                    }
                }
            }

            // Filter available quests
            var availableQuests = quests
                .Where(q => !usedIds.Contains(q.Id))
                .ToList();

            // If all quests used → reset
            if (availableQuests.Count == 0)
            {
                _logger.LogInformation("All quests used. Resetting history.");
                usedIds.Clear();
                availableQuests = quests;
            }

            _chosenSideQuest = availableQuests[_random.Next(availableQuests.Count)];

            var currentQuest = new
            {
                Date = DateTime.UtcNow,
                Quest = _chosenSideQuest
            };

            var currentJson = JsonSerializer.Serialize(currentQuest, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_currentQuestFile, currentJson);

            var outputLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - Chosen Quest ID: {_chosenSideQuest.Id}";

            await File.AppendAllTextAsync(_historyFile, outputLine + Environment.NewLine);

            _logger.LogInformation($"Selected quest of the week {_chosenSideQuest.Id}: {_chosenSideQuest.Title}");
        }
    }
}
