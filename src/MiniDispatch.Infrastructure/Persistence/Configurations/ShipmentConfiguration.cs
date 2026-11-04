using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniDispatch.Domain.Entities;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Infrastructure.Persistence.Configurations
{
    public sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
             builder.ToTable("Shipments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.HasIndex(x => x.ReferenceNumber)
                .IsUnique();

            builder.Property(x => x.Origin)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Destination)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.VehiclePlate)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasData(new
            {
                Id = Guid.Parse("b348a8d5-e8ea-47d7-b9a4-5a4f0308f080"),
                ReferenceNumber = "SHP-1001",
                Origin = "İstanbul",
                Destination = "Ankara",
                VehiclePlate = "34 ABC 123",
                Status = ShipmentStatus.Created,
                CreatedAtUtc = new DateTime(
                    2026, 11, 4, 9, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}