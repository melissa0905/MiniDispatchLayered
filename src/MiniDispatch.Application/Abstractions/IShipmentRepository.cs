using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniDispatch.Domain.Entities;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Application.Abstractions
{
    public interface IShipmentRepository
    {
        Task<IReadOnlyList<Shipment>> GetListAsync(
       ShipmentStatus? status,
       CancellationToken cancellationToken);
        Task<bool> ReferenceNumberExistsAsync(
       string referenceNumber,
       CancellationToken cancellationToken);

        Task AddAsync(
            Shipment shipment,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}