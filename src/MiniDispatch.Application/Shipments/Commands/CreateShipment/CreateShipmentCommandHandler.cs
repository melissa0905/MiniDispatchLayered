using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Domain.Entities;

namespace MiniDispatch.Application.Shipments.Commands.CreateShipment
{
    public class CreateShipmentCommandHandler(IShipmentRepository shipmentRepository) : IRequestHandler<CreateShipmentCommand, Guid>
    {
        public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            Validator.ValidateObject(
        request,
        new ValidationContext(request),
        validateAllProperties: true);

            var referenceNumber = request.ReferenceNumber.Trim();

            var referenceExists =
                await shipmentRepository.ReferenceNumberExistsAsync(
                    referenceNumber,
                    cancellationToken);

            if (referenceExists)
            {
                throw new ValidationException(
                    "Bu referans numarasıyla bir sevkiyat zaten mevcut.");
            }

            var shipment = new Shipment(
                referenceNumber,
                request.Origin,
                request.Destination,
                request.VehiclePlate);

            await shipmentRepository.AddAsync(
                shipment,
                cancellationToken);

            await shipmentRepository.SaveChangesAsync(
                cancellationToken);

            return shipment.Id;
        }
    }
}