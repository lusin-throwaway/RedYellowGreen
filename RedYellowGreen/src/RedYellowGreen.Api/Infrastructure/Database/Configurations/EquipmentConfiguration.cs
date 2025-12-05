using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Infrastructure.Database.Configurations;

internal class EquipmentConfiguration : IEntityTypeConfiguration<EquipmentEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentEntity> builder)
    {
        builder.OwnsOne(
            x => x.CurrentState,
            ownedBuilder => { ownedBuilder.HasIndex(x => x.State); }
        );
    }
}

internal class EquipmentStatesConfiguration : IEntityTypeConfiguration<EquipmentStateEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentStateEntity> builder)
    {
        builder
            .HasOne(x => x.Equipment)
            .WithMany(e => e.States)
            .HasForeignKey(x => x.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.State);
    }
}