using System.ComponentModel.DataAnnotations;

namespace CompetitionAPI.DTO
{
    public class RegisterStudentDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Class { get; set; } = string.Empty;
        [Required]
        public string Section { get; set; } = string.Empty;
        [Required]
        public string House { get; set; } = string.Empty;
        [Required]
        public string NcpscId { get; set; } = string.Empty;
    }
}
