using CompetitionAPI.Models;

namespace CompetitionAPI.DTO
{
    public class StudentWithMarkDTO : Student
    {
        public double Score { get; set; }
        public string SchoolName { get; set; } = string.Empty;
    }
}
