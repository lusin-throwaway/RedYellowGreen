using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedYellowGreen.Api.Features.Orders.Models;

namespace RedYellowGreen.Api.Infrastructure.Database.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder
            .HasOne(order => order.Equipment)
            .WithMany(equipment => equipment.Orders)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status);
    }
}