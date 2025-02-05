using DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<BookEntity> Books { get; set; }
        public DbSet<AuthorEntity> Authors { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuthorEntity>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("uuid_generate_v4()"); 

            modelBuilder.ApplyConfiguration(new BookEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        }
    }
}