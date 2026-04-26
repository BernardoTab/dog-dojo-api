namespace dog_dojo_backend.Entities
{
    public class CompletedQuest
    {
        public long Id { get; set; }
        public DateTime CompletedDate { get; set; }
        public long QuestId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
