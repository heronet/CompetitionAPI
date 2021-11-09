using CompetitionAPI.Data;
using CompetitionAPI.DTO;
using CompetitionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionAPI.Controllers
{
    public class StudentsController : DefaultController
    {
        ApplicationDbContext _dbcontext;
        public StudentsController(ApplicationDbContext dbContext) => _dbcontext = dbContext;

        public async Task<IActionResult> AddStudent(RegisterStudentDTO studentDTO)
        {
            var student = new Student
            {
                Name = studentDTO.Name.Trim(),
                Class = studentDTO.Class.Trim(),
                Section = studentDTO.Section.Trim(),
                Email = studentDTO.Email.Trim(),
                Phone = studentDTO.Phone.Trim(),
                House = studentDTO.House.Trim(),
                NcpscId = studentDTO.NcpscId.Trim().ToLower(),
            };
            var exists = await _dbcontext.Students.Where(x => x.NcpscId == student.NcpscId).FirstOrDefaultAsync();
            if (exists != null) return BadRequest("[Error]: Student Already Exists");

            _dbcontext.Students.Add(student);
            if (await _dbcontext.SaveChangesAsync() > 0)
                return Ok();
            return BadRequest("Couldn't add Student");
        }
        public async Task<ActionResult<GetResponseWithPageDTO<Student>>> GetStudents(int pageSize = 10, int pageNumber = 1)
        {
            var students = await _dbcontext.Students.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            var studentsDtos = students.Select(x => x.Competitions = null);
            return Ok(studentsDtos);
        }
    }
}
