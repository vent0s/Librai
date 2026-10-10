using LibrAI.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace LibrAI.Infrastructure.Persistence
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {

        }

        public DbSet<Title> Titles => Set<Title>();
        public DbSet<Copy> Copies => Set<Copy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Copy>()                     //we configurate rule for Copy
                .HasOne(c => c.Title)                       //which has attribute associate to a Title
                .WithMany()                                 //and a Title which is associate to multiple Copies
                .HasForeignKey(c => c.TitleId)              //Hence, TitleId is foreign key for Copies
                .IsRequired()                               //and which is required for copies
                .OnDelete(DeleteBehavior.Restrict);         //when delete, we need to perform restrict mode
        }

    }
}