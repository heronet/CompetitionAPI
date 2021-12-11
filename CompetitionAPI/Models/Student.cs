namespace CompetitionAPI.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string House { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NcpscId { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public School? School { get; set; } = null;
        public Guid SchoolId { get; set; }
        public ICollection<Competition>? Competitions { get; set; } = new List<Competition>();
    }
}
