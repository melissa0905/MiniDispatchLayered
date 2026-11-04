using System.ComponentModel.DataAnnotations;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Application.Shipments.Commands.ChangeShipmentStatus;
using MiniDispatch.Application.Shipments.Commands.CreateShipment;
using MiniDispatch.Domain.Entities;
using MiniDispatch.Domain.Enums;

var repository = new FakeRepository();
var notifier = new FakeNotifier(repository);
var create = new CreateShipmentCommandHandler(repository, notifier);
var change = new ChangeShipmentStatusCommandHandler(repository, notifier);
var id = await create.Handle(new CreateShipmentCommand
{
    ReferenceNumber = "TEST-1", Origin = "Istanbul", Destination = "Ankara", VehiclePlate = "34 ABC 123"
}, default);
Assert(notifier.Count == 1 && repository.Shipment?.Id == id, "Create must save then notify.");

await change.Handle(new(id, ShipmentStatus.InTransit), default);
Assert(notifier.Count == 2 && repository.Shipment!.Status == ShipmentStatus.InTransit,
    "Status change must save then notify.");

await Expect<ValidationException>(() => change.Handle(new(id, ShipmentStatus.InTransit), default));
await Expect<ValidationException>(() => change.Handle(new(id, (ShipmentStatus)999), default));
await Expect<ValidationException>(() => change.Handle(new(Guid.Empty, ShipmentStatus.Delivered), default));
await Expect<KeyNotFoundException>(() => change.Handle(new(Guid.NewGuid(), ShipmentStatus.Delivered), default));
Assert(notifier.Count == 2, "Invalid commands must not notify.");

repository.FailSave = true;
await Expect<InvalidOperationException>(() => change.Handle(new(id, ShipmentStatus.Delivered), default));
Assert(notifier.Count == 2, "Failed commits must not notify.");
Console.WriteLine("PASS: creation, status change, invalid commands, missing shipment, failed save, and notification order.");

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static async Task Expect<T>(Func<Task> action) where T : Exception
{
    try { await action(); }
    catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}");
}

sealed class FakeRepository : IShipmentRepository
{
    public Shipment? Shipment { get; private set; }
    public int Saves { get; private set; }
    public bool FailSave { get; set; }
    public Task AddAsync(Shipment shipment, CancellationToken cancellationToken)
    {
        Shipment = shipment;
        return Task.CompletedTask;
    }
    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Shipment?.Id == id ? Shipment : null);
    public Task<bool> ReferenceNumberExistsAsync(string referenceNumber, CancellationToken cancellationToken) =>
        Task.FromResult(false);
    public Task<IReadOnlyList<Shipment>> GetListAsync(ShipmentStatus? status, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (FailSave) throw new InvalidOperationException("Simulated database failure");
        Saves++;
        return Task.CompletedTask;
    }
}

sealed class FakeNotifier(FakeRepository repository) : IShipmentNotifier
{
    public int Count { get; private set; }
    public Task ShipmentsChangedAsync(CancellationToken cancellationToken)
    {
        if (repository.Saves != Count + 1) throw new Exception("Notification preceded database commit.");
        Count++;
        return Task.CompletedTask;
    }
}
