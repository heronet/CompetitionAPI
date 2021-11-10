using CompetitionAPI.Data;
using CompetitionAPI.DTO;
using CompetitionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompetitionAPI.Utilities.Constants;

namespace CompetitionAPI.Controllers
{
    [Authorize(Policy = Policies.AccessCompetitions)]
    public class CompetitionController : DefaultController
    {
        ApplicationDbContext _dbcontext;
        public CompetitionController(ApplicationDbContext dbContext) => _dbcontext = dbContext;

        [HttpGet]
        public async Task<ActionResult<GetResponseWithPageDTO<Competition>>> GetCompetitions(int pageSize = 10, int pageNumber = 1)
        {
            var competitions = await _dbcontext.Competitions
                .OrderByDescending(x => x.CreatedAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return Ok(new GetResponseWithPageDTO<Competition>(competitions, competitions.Count));
        }

        [HttpPost]
        public async Task<IActionResult> AddCompetition(AddCompetitionDTO competitionDTO)
        {
            var competition = new Competition
            {
                Name = competitionDTO.Name,
                DateTime = competitionDTO.DateTime
            };
            _dbcontext.Competitions.Add(competition);

            if (await _dbcontext.SaveChangesAsync() > 0)
                return Ok();
            return BadRequest("An Error Occured");
        }
    }
}