using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MiniDispatch.Application.Abstractions;

namespace MiniDispatch.Application.Shipments.Queries.GetShipments
{
    public sealed class GetShipmentsQueryHandler(
        IShipmentRepository shipmentRepository)
        : IRequestHandler<GetShipmentsQuery, IReadOnlyList<ShipmentListItemDto>>
    {
        public async Task<IReadOnlyList<ShipmentListItemDto>> Handle(
            GetShipmentsQuery request,
            CancellationToken cancellationToken)
        {
            var shipments = await shipmentRepository.GetListAsync(
                request.Status,
                cancellationToken);

            return shipments.Select(s => new ShipmentListItemDto(
                s.Id,
                s.ReferenceNumber,
                s.Origin,
                s.Destination,
                s.VehiclePlate,
                s.Status)).ToList();
        }
    }
}