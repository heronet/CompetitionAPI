using System.Collections.Generic;

namespace CompetitionAPI.DTO
{
    public class TeacherAuthDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}