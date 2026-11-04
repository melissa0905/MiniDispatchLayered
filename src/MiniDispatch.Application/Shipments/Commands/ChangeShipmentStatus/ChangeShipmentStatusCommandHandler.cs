using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Application.Shipments.Commands.ChangeShipmentStatus
{
    public class ChangeShipmentStatusCommandHandler(
    IShipmentRepository shipmentRepository,
    IShipmentNotifier shipmentNotifier)
    : IRequestHandler<ChangeShipmentStatusCommand>
    {
        
    public async Task Handle(
        ChangeShipmentStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ValidationException(
                "Geçerli bir sevkiyat seçilmelidir.");
        }

        if (request.Status != ShipmentStatus.InTransit &&
            request.Status != ShipmentStatus.Delivered)
        {
            throw new ValidationException(
                "Geçersiz hedef sevkiyat durumu.");
        }

        var shipment = await shipmentRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (shipment is null)
        {
            throw new KeyNotFoundException(
                "Sevkiyat bulunamadı.");
        }

        try
        {
            switch (request.Status)
            {
                case ShipmentStatus.InTransit:
                    shipment.StartTransit();
                    break;

                case ShipmentStatus.Delivered:
                    shipment.Deliver();
                    break;
            }
        }
        catch (InvalidOperationException exception)
        {
            throw new ValidationException(exception.Message);
        }

        await shipmentRepository.SaveChangesAsync(
            cancellationToken);

        await shipmentNotifier.ShipmentsChangedAsync(CancellationToken.None);
    }
    }
}
