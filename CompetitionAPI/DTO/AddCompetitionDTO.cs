using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompetitionAPI.DTO
{
    public class AddCompetitionDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }
}