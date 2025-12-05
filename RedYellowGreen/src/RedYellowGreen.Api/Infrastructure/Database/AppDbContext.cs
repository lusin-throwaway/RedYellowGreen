using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Infrastructure.Database;

public class AppDbContext : DbContext
{
    internal DbSet<EquipmentEntity> Equipment { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>()
            .HaveMaxLength(255);
    }
}