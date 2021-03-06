using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CompetitionAPI.DTO
{
    public class AddCompetitionDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime DateTime { get; set; } = DateTime.UtcNow.AddHours(6.00);
    }
}