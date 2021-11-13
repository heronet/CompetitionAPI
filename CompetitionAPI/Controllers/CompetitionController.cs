using CompetitionAPI.Data;
using CompetitionAPI.DTO;
using CompetitionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompetitionAPI.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CompetitionAPI.Controllers
{
    [Authorize(Policy = Policies.AccessCompetitions)]
    public class CompetitionController : DefaultController
    {
        ApplicationDbContext _dbcontext;
        private UserManager<Teacher> _userManager;

        public CompetitionController(
            ApplicationDbContext dbContext, 
            UserManager<Teacher> userManager
        )
        {
            _dbcontext = dbContext;
            _userManager = userManager;
        }

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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetition(Guid id)
        {
            var competition = await _dbcontext.Competitions.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (competition == null) return BadRequest("[Error]: Competition With ID Not Found");

            _dbcontext.Competitions.Remove(competition);
            if (await _dbcontext.SaveChangesAsync() > 0) return Ok();
            return BadRequest("An Error Occured");
        }
        [HttpPost("{competitionId}/add")]
        public async Task<IActionResult> AddParticipant(Guid competitionId, AddParticipantDTO participantDTO)
        {
            Console.WriteLine(participantDTO.studentId);
            var competition = await _dbcontext.Competitions.Where(x => x.Id == competitionId).FirstOrDefaultAsync();
            if (competition == null) return BadRequest("[Error]: Unknown Competition");

            var student = await _dbcontext.Students.Where(x => x.Id == participantDTO.studentId).FirstOrDefaultAsync();
            if (student == null) return BadRequest("[Error]: Unknown Student");

            competition.Attendees!.Add(student);

            if (await _dbcontext.SaveChangesAsync() > 0)
                return Ok();
            return BadRequest("An Error Occured");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetResponseWithPageDTO<Student>>> GetCompetitionParticipants(Guid id, int pageSize = 10, int pageNumber = 1)
        {
            var competition = await _dbcontext.Competitions
                .Where(x => x.Id == id)
                .Include(x => x.Attendees!
                    .OrderByDescending(x => x.NcpscId)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                 )
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (competition == null) return BadRequest("[Error]: Unknown Competition");

            var participants = competition.Attendees!.ToList();
            foreach(var part in participants)
                part.Competitions = null;
            return Ok(new GetResponseWithPageDTO<Student>(participants, participants.Count));
        }

        [HttpGet("student/{id}")]
        public async Task<ActionResult<GetResponseWithPageDTO<Student>>> GetStudentCompetitions(Guid id, int pageSize = 10, int pageNumber = 1)
        {
            var student = await _dbcontext.Students
                .Where(x => x.Id == id)
                .Include(x => x.Competitions!
                    .OrderByDescending(x => x.DateTime)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                 )
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (student == null) return BadRequest("[Error]: Unknown Student");

            var competitions = student.Competitions!.ToList();
            foreach(var competition in competitions)
                competition.Attendees = null;
            return Ok(new GetResponseWithPageDTO<Competition>(competitions, competitions.Count));
        }

        // Marks
        [HttpPost("{competitionId}/mark")]
        public async Task<IActionResult> MarkParticipant(Guid competitionId, MarkParticipantDTO markParticipantDTO)
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var teacher = await _userManager.FindByIdAsync(id);
            if (teacher == null) return BadRequest("[Error]: Invalid Teacher ID");

            var competition = await _dbcontext.Competitions
                .Where(x => x.Id == competitionId)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync();

            if (competition == null) return BadRequest("[Error]: Invalid Competition ID");

            var student = competition.Attendees!.Where(x => x.Id == markParticipantDTO.StudentId).FirstOrDefault();
            if (student == null) return BadRequest("[Error]: Student's Not Competing");

            var score = new TeacherStudentCompetition
            {
                Teacher = teacher,
                Student = student,
                Competition = competition,
                Marks = markParticipantDTO.Score
            };
            _dbcontext.TscCollection.Add(score);
            if (await _dbcontext.SaveChangesAsync() > 0) return Ok();

            return BadRequest("An Error Occured");
        }
    }
}