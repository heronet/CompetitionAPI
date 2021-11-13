namespace CompetitionAPI.Models
{
    public class TeacherStudentCompetition
    {
        public Guid Id { get; set; }
        public double Marks { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid CompetitionId { get; set; }
        public Competition? Competition { get; set; }
    }
}
