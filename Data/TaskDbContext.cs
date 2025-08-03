using Microsoft.EntityFrameworkCore;
using TodoApi.Models; // ייבוא המודל
// using Microsoft.EntityFrameworkCore;

namespace TodoApi.Data
{
   public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) {}

        public DbSet<TaskItem> Tasks { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseMySql("server=localhost;database=Items;user=root;password=YourPassword;",
        //             new MySqlServerVersion(new Version(8, 0, 36))); // יש לוודא שהגרסה נכונה לפי הגרסה שלך
        //     }
        // }
    }
}
