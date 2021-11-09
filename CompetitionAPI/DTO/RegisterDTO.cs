using System.ComponentModel.DataAnnotations;

namespace CompetitionAPI.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string NcpscId { get; set; } = string.Empty;
    }
}