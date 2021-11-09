using System.ComponentModel.DataAnnotations;

namespace CompetitionAPI.DTO
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}