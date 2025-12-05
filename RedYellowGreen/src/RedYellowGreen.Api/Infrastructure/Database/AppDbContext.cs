using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Infrastructure.Database;

public class AppDbContext : DbContext
{
    internal DbSet<EquipmentEntity> Equipment { get; set; }
}