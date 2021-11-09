namespace CompetitionAPI.Models
{
    public class Competition
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public ICollection<Student>? Attendees { get; set; }
    }
}
