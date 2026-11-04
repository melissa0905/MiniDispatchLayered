using MediatR;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Application.Shipments.Queries.GetShipments;

public sealed record GetShipmentsQuery(
    ShipmentStatus? Status = null)
    : IRequest<IReadOnlyList<ShipmentListItemDto>>;