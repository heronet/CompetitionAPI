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

        [HttpGet("{competitionId}")]
        public async Task<ActionResult<GetResponseWithPageDTO<Student>>> GetCompetitionParticipants(Guid competitionId, int pageSize = 10, int pageNumber = 1)
        {
            var competition = await _dbcontext.Competitions
                .Where(x => x.Id == competitionId)
                .Include(x => x.Attendees!
                    .OrderByDescending(x => x.NcpscId)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                 )
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (competition == null) return BadRequest("[Error]: Unknown Competition");

            var participants = competition.Attendees!.Select(x => StudentToDTO(x)).ToList();

            foreach(var part in participants)
                part.Competitions = null;

            // Check if teacher has marked the student already
            string teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var teacher = await _userManager.FindByIdAsync(teacherId);
            if (teacher == null) return BadRequest("[Error]: Invalid Teacher ID");

            var competitionsWithTeacher = await _dbcontext.TscCollection
                .Where(tsc => tsc.CompetitionId == competitionId && tsc.TeacherId == teacherId)
                .ToListAsync();
            foreach (var competitionWithTeacher in competitionsWithTeacher)
            {
                var partMod = participants.Where(p => p.Id == competitionWithTeacher.StudentId).FirstOrDefault();
                if(partMod != null) partMod.Score = competitionWithTeacher.Marks;
            }

            return Ok(new GetResponseWithPageDTO<StudentWithMarkDTO>(participants, participants.Count));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{competitionId}/scores")]
        public async Task<ActionResult<GetResponseWithPageDTO<Student>>> GetCompetitionParticipantsScores(Guid competitionId, int pageSize = 10, int pageNumber = 1)
        {
            var competition = await _dbcontext.Competitions
                .Where(x => x.Id == competitionId)
                .Include(x => x.Attendees!
                    .OrderByDescending(x => x.NcpscId)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                 )
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (competition == null) return BadRequest("[Error]: Unknown Competition");

            var participants = competition.Attendees!.Select(x => StudentToDTO(x)).ToList();

            foreach(var part in participants)
                part.Competitions = null;

            var results = await _dbcontext.TscCollection
                .Where(tsc => tsc.CompetitionId == competitionId)
                .OrderByDescending(tsc => tsc.Marks)
                .ToListAsync();
            foreach (var result in results)
            {
                var partMod = participants.Where(p => p.Id == result.StudentId).FirstOrDefault();
                if (partMod != null) { partMod.Score += result.Marks; }
            }

            return Ok(new GetResponseWithPageDTO<StudentWithMarkDTO>(participants, participants.Count));
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

        // Mark
        [HttpPost("{competitionId}/mark")]
        public async Task<IActionResult> MarkParticipant(Guid competitionId, MarkParticipantDTO markParticipantDTO)
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var teacher = await _userManager.FindByIdAsync(id);
            if (teacher == null) return BadRequest("[Error]: Invalid Teacher ID");

            // Check if teacher marked the student already
            var competitionWithTeacherAndStudent = await _dbcontext.TscCollection
                .Where(tsc => tsc.CompetitionId == competitionId && tsc.TeacherId == id && tsc.StudentId == markParticipantDTO.StudentId)
                .FirstOrDefaultAsync();
            if (competitionWithTeacherAndStudent != null) return BadRequest("[Error]: Already marked Student");

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

        private StudentWithMarkDTO StudentToDTO(Student student)
        {
            return new StudentWithMarkDTO
            {
                Name = student.Name,
                Phone = student.Phone,
                NcpscId = student.NcpscId,
                House = student.House,
                Class = student.Class,
                Email = student.Email,
                Section = student.Section,
                Id = student.Id
            };
        }
    }
}