using CompetitionAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompetitionAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<Teacher, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<TeacherStudentCompetition> TscCollection { get; set; }
    }
}