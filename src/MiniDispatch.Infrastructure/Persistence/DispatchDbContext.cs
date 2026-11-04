using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniDispatch.Domain.Entities;

namespace MiniDispatch.Infrastructure.Persistence
{
    public sealed class DispatchDbContext(
     DbContextOptions<DispatchDbContext> options)
     : DbContext(options)
    {
        public DbSet<Shipment> Shipments => Set<Shipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dispatch");

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(DispatchDbContext).Assembly);
        }
    }
}