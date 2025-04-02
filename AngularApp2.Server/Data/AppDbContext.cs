using AngularApp2.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularApp2.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Addresses)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
