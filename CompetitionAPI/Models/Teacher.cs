using Microsoft.AspNetCore.Identity;

namespace CompetitionAPI.Models
{
    public class Teacher : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string NcpscId { get; set; } = string.Empty;
    }
}
