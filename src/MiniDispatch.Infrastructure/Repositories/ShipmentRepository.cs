using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Domain.Entities;
using MiniDispatch.Domain.Enums;
using MiniDispatch.Infrastructure.Persistence;

namespace MiniDispatch.Infrastructure.Repositories
{
    public sealed class ShipmentRepository(DispatchDbContext dbContext) : IShipmentRepository
    {
        public async Task<IReadOnlyList<Shipment>> GetListAsync(ShipmentStatus? status, CancellationToken cancellationToken)
        {
            var query = dbContext.Shipments
                    .AsNoTracking()
                    .AsQueryable();


            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }


            return await query.OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(100)
            .ToListAsync(cancellationToken);
        }
    }
}