using RuslanMurzaevKt_31_23.Database.Configurations;
using RuslanMurzaevKt_31_23.Models;
using Microsoft.EntityFrameworkCore;

namespace RuslanMurzaevKt_31_23.Database
{
    public class StudentDbContext : DbContext
    {
        DbSet<Student> Students { get; set; }
        DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
        }

        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }
    }
}
