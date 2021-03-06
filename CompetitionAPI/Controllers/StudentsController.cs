using CompetitionAPI.Data;
using CompetitionAPI.DTO;
using CompetitionAPI.Models;
using CompetitionAPI.Utilities.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionAPI.Controllers
{
    [Authorize(Policy = Policies.AccessStudents)]
    public class StudentsController : DefaultController
    {
        ApplicationDbContext _dbcontext;
        public StudentsController(ApplicationDbContext dbContext) => _dbcontext = dbContext;

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddStudent(RegisterStudentDTO studentDTO)
        {
            // Scheck if school Exists
            var school = await _dbcontext.Schools
                .Where(s => s.Name == studentDTO.School.Trim())
                .FirstOrDefaultAsync();
            if (school == null)
                return BadRequest("School Not Found");
            var student = new Student
            {
                Name = studentDTO.Name.Trim(),
                Class = studentDTO.Class.Trim(),
                Section = studentDTO.Section.Trim(),
                Email = studentDTO.Email.Trim(),
                Phone = studentDTO.Phone.Trim(),
                House = studentDTO.House.Trim(),
                NcpscId = studentDTO.NcpscId.Trim().ToLower(),
                School = school
            };
            var exists = await _dbcontext.Students
                .Where(x => x.NcpscId.ToLower() == student.NcpscId.ToLower())
                .FirstOrDefaultAsync();
            if (exists != null) return BadRequest("[Error]: Student With Same ID Already Exists");

            _dbcontext.Students.Add(student);
            if (await _dbcontext.SaveChangesAsync() > 0)
                return Ok();
            return BadRequest("Couldn't add Student");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _dbcontext.Students.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (student == null) return BadRequest("[Error]: Student With ID Not Found");

            _dbcontext.Students.Remove(student);
            if (await _dbcontext.SaveChangesAsync() > 0) return Ok();
            return BadRequest("An Error Occured");
        }
        [HttpGet]
        public async Task<ActionResult<GetResponseWithPageDTO<Student>>> GetStudents(int pageSize = 1000, int pageNumber = 1)
        {
            var students = await _dbcontext.Students
                .OrderBy(x => x.NcpscId)
                .Include(s => s.School)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            foreach (var student in students)
                student.School!.Students = null;
            return Ok(new GetResponseWithPageDTO<Student>(students, students.Count));
        }
        [HttpPost("add-school")]
        public async Task<IActionResult> AddSchool(NewSchoolDTO schoolDTO)
        {
            var school = new School
            {
                Name = schoolDTO.Name.Trim()
            };
            _dbcontext.Schools.Add(school);
            if (await _dbcontext.SaveChangesAsync() > 0)
                return Ok("Added Successfully");
            return BadRequest("Failed To add school");
        }
    }
}
