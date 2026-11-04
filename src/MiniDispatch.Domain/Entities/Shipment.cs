using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Domain.Entities
{
    public sealed class Shipment
    {
        public Guid Id { get; private set; }
        public string ReferenceNumber { get; private set; } = string.Empty;
        public string Origin { get; private set; } = string.Empty;
        public string Destination { get; private set; } = string.Empty;
        public string VehiclePlate { get; private set; } = string.Empty;
        public ShipmentStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        // EF Core için.
        private Shipment()
        {
        }

        public Shipment(
            string referenceNumber,
            string origin,
            string destination,
            string vehiclePlate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            ArgumentException.ThrowIfNullOrWhiteSpace(destination);
            ArgumentException.ThrowIfNullOrWhiteSpace(vehiclePlate);

            Id = Guid.NewGuid();
            ReferenceNumber = referenceNumber.Trim();
            Origin = origin.Trim();
            Destination = destination.Trim();
            VehiclePlate = vehiclePlate.Trim();
            Status = ShipmentStatus.Created;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void StartTransit()
        {
            if (Status != ShipmentStatus.Created)
            {
                throw new InvalidOperationException(
                    "Yalnızca oluşturulmuş sevkiyat yola çıkarılabilir.");
            }

            Status = ShipmentStatus.InTransit;
        }

        public void Deliver()
        {
            if (Status != ShipmentStatus.InTransit)
            {
                throw new InvalidOperationException(
                    "Yalnızca yoldaki sevkiyat teslim edilebilir.");
            }

            Status = ShipmentStatus.Delivered;
        }
    }
}