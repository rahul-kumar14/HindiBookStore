using HindiBookStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HindiBookStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "कहानी (Story)" },
                new Category { Id = 2, Name = "उपन्यास (Novel)" },
                new Category { Id = 3, Name = "कविता (Poetry)" },
                new Category { Id = 4, Name = "आत्मकथा (Autobiography)" },
                new Category { Id = 5, Name = "नाटक (Drama)" }
            );
        }
    }
}
