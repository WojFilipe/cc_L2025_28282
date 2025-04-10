using Microsoft.EntityFrameworkCore;

namespace Cdv.PeopleApi
{
    public class PeopleDb : DbContext
    {
        public PeopleDb(DbContextOptions<PeopleDb> options) : base(options)
        {
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var PersonEntity = modelBuilder.Entity<PersonEntity>();

            PersonEntity.ToTable("Person");
            PersonEntity.HasKey(i => i.Id);

            PersonEntity.Property(p => p.FirstName).HasMaxLength(250);
            PersonEntity.Property(p => p.LastName).HasMaxLength(250);
            PersonEntity.Property(p => p.PhoneNumber).HasMaxLength(12);
        }
    
        public DbSet<PersonEntity> People { get; set; } 
    
    }
}
