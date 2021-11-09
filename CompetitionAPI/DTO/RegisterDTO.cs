using System.ComponentModel.DataAnnotations;

namespace CompetitionAPI.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string NcpscId { get; set; } = string.Empty;
    }
}