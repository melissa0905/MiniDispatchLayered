using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Application.Shipments.Queries.GetShipments;

public sealed record ShipmentListItemDto(
    Guid Id,
    string ReferenceNumber,
    string Origin,
    string Destination,
    string VehiclePlate,
    ShipmentStatus Status);
