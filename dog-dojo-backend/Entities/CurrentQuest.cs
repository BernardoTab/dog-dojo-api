namespace dog_dojo_backend.Entities
{
    public class CurrentQuest
    {
        public long Id { get; set; }
        public long QuestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate {  get; set; }
    }
}
