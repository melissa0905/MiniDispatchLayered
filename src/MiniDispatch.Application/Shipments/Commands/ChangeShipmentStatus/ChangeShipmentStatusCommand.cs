using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Application.Shipments.Commands.ChangeShipmentStatus
{
public sealed record ChangeShipmentStatusCommand(
    Guid Id,
    ShipmentStatus Status) : IRequest;
}