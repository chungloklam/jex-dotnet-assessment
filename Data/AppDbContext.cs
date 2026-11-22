using JexAssessment.Models;
using Microsoft.EntityFrameworkCore;

namespace JexAssessment.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(v => v.Description)
                .HasMaxLength(4000);

            entity.HasOne(v => v.Company)
                .WithMany(c => c.Vacancies)
                .HasForeignKey(v => v.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}