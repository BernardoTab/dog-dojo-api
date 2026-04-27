namespace dog_dojo_backend.Entities
{
    public class CompletedQuest
    {
        public long Id { get; set; }
        public DateTimeOffset CompletedDate { get; set; }
        public long QuestId { get; set; }
        public DateTimeOffset StartDate { get; set; }
    }
}
