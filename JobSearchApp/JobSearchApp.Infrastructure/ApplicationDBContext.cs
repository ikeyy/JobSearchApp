using JobSearchApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobSearchApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Job> Job { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Contractor> Contractor { get; set; }

        public DbSet<JobOffer> JobOffer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobOffer>()
                .HasOne(j => j.Contractor)
                .WithMany()
                .HasForeignKey(j => j.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
