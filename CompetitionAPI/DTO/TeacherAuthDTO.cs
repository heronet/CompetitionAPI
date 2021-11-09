using System.Collections.Generic;

namespace CompetitionAPI.DTO
{
    public class TeacherAuthDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}